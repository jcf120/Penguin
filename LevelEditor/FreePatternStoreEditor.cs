using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	
	public class FreePatternStoreEditor : PatternArrayViewDataSource, PatternArrayViewResponder
	{
		private PatternArrayView patternView_;
		
		// Store selection and linking
		private Dictionary<string, FreePatternStore> storesDict_;
		public  Dictionary<string, FreePatternStore> storesDict {get{return storesDict_;}}
		
		// File tracking
		enum FileState {unsaved, saved, edited}
		private Dictionary<string, FileState> fileStates_;
		
		
		// Serialized data
		private SerializedObject   serializedTarget_;
		private SerializedProperty width_;
		private SerializedProperty height_;
		private SerializedProperty values_;
		private SerializedProperty valuesCount_;
		
		
		// New store GUI
		private string newStoreName_ = "untitled";
		
		
		// Save/load directory
		private static string storeDirectory_ = "Assets/Levels/FreePatternStores/";
		
		
		// Cell paint brushes
		string[]   cellTypeNames_;
		CellType[] cellTypes_;
		int        leftClickCellTypeIndex_;
		int        rightClickCellTypeIndex_;
		
		
		public FreePatternStoreEditor()
		{
			storesDict_ = new Dictionary<string, FreePatternStore>();
			fileStates_ = new Dictionary<string, FileState>();
			
			patternView_ = new PatternArrayView(this);
			// Register for click input
			patternView_.responder = this;
			
			// Setup cellpainting palette
			// Collect possible values excluding undefined
			cellTypes_     = (from CellType ct in Enum.GetValues(typeof(CellType))
							  where (ct != CellType.Undefined)
							  select ct).ToArray();
			cellTypeNames_ = (from CellType ct in cellTypes_
							  select Enum.GetName(typeof(CellType), ct)).ToArray();
			leftClickCellTypeIndex_  = (int)Array.IndexOf<CellType>(cellTypes_, CellType.Normal);
			rightClickCellTypeIndex_ = (int)Array.IndexOf<CellType>(cellTypes_, CellType.Empty);
		}
		
		
		#region GUI Code
		
		
		public void OnGUI ()
		{
			EditorGUILayout.BeginHorizontal();
			
			patternView_.OnGUI();
			
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(250.0f));
			loadStoresDragAndDropGUI();
			newStoreGUI();
			storeSelectionAndSavingGUI();
			
			bool oldEnabled = GUI.enabled;
			GUI.enabled = oldEnabled && serializedTarget_ != null;
			storeBoundsGUI();
			GUI.enabled = oldEnabled;
			
			cellTypePaletteGUI();
			
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.EndHorizontal();
			
		}
		
		
		private void loadStoresDragAndDropGUI()
		{
			var evt = Event.current;
			
			var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
			GUI.Box(dropArea, "Drop to load stores");
			
			switch(evt.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				// Ignore event if outside drop area
				if (!dropArea.Contains(evt.mousePosition))
					break;
				
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				
				if (evt.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag();
					
					foreach (var draggedObject in DragAndDrop.objectReferences) {
						
						var textAsset = draggedObject as TextAsset;
						if (!textAsset)
							continue;
						
						loadStore(textAsset);
					}
				}
				break;
			}
		}
		
		
		private void newStoreGUI()
		{
			EditorGUILayout.BeginHorizontal();
			
			newStoreName_ = GUILayout.TextField(newStoreName_);
			// Enable save if text has been entered
			bool oldEnabled = GUI.enabled; // state-machine housekeeping
			GUI.enabled = (newStoreName_.Length > 0) && oldEnabled;
			if (GUILayout.Button("New Store"))
				newStore(newStoreName_);
			GUI.enabled = oldEnabled;
			
			EditorGUILayout.EndHorizontal();
		}
		
		
		private void storeSelectionAndSavingGUI()
		{
			// Convert dictionary to array of names and find selected index
			List<string> storeNames = (from kvp in storesDict select kvp.Key).ToList();
			// Append file states
			List<string>storeLabels = (from s
									   in storeNames
									   select s +  " - " + fileStates_[s].ToString()).ToList();
			
			int selIndex;
			if (serializedTarget_ != null) { // selection present
				string selName = nameForStore((FreePatternStore)serializedTarget_.targetObject);
				selIndex = storeNames.IndexOf(selName);
				
			} else { // no selection
				storeLabels.Add("-");
				selIndex = storeLabels.IndexOf("-");
			}
			
			EditorGUILayout.BeginHorizontal();
			
			int newSelIndex = EditorGUILayout.Popup(selIndex, storeLabels.ToArray());
			
			// Detect selection change
			if (newSelIndex != selIndex)
				selectStore(storeNames[newSelIndex]);
			
			// Enable save for valid selection
			bool oldEnabled = GUI.enabled; // state-machine housekeeping
			GUI.enabled = serializedTarget_!=null && oldEnabled;
			if (GUILayout.Button("Save"))
				saveStore(storeNames[newSelIndex]);
			GUI.enabled = oldEnabled;
			
			EditorGUILayout.EndHorizontal();
		}
		
		
		private void storeBoundsGUI()
		{
			bool oldEnabled = GUI.enabled;
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Columns:", GUILayout.Width(55.0f));
			
			EditorGUILayout.LabelField("Left", GUILayout.Width(35.0f));
			if (GUILayout.Button("+"))
				addColumnLeft();
			GUI.enabled = oldEnabled && width_.intValue > 1;
			if (GUILayout.Button("-"))
				removeColumnLeft();
			GUI.enabled= oldEnabled;
			
			EditorGUILayout.LabelField("Right", GUILayout.Width(35.0f));
			if (GUILayout.Button("+"))
				addColumnRight();
			GUI.enabled = oldEnabled && width_.intValue > 1;
			if (GUILayout.Button("-"))
				removeColumnRight();
			GUI.enabled = oldEnabled;
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Rows", GUILayout.Width(35.0f));
			if (GUILayout.Button("+"))
				addRowTop();
			GUI.enabled = oldEnabled && height_.intValue > 1;
			if (GUILayout.Button("-"))
				removeRowTop();
			GUI.enabled = oldEnabled;
			EditorGUILayout.EndHorizontal();
		}
		
		
		private void cellTypePaletteGUI()
		{
			EditorGUILayout.LabelField("Paint Brush:");
			EditorGUILayout.BeginHorizontal();
			leftClickCellTypeIndex_  = EditorGUILayout.Popup(leftClickCellTypeIndex_,  cellTypeNames_);
			rightClickCellTypeIndex_ = EditorGUILayout.Popup(rightClickCellTypeIndex_, cellTypeNames_);
			EditorGUILayout.EndHorizontal();
		}
		
		
		#endregion GUI Code
		
		
		#region Array Handling
		
		
		private void setTarget(FreePatternStore store)
		{
			serializedTarget_ = new SerializedObject(store);
			
			width_       = serializedTarget_.FindProperty("width" );
			height_      = serializedTarget_.FindProperty("height");
			values_      = serializedTarget_.FindProperty("values");
			valuesCount_ = serializedTarget_.FindProperty("values.Array.size");
		}
		
		
		public void selectStore(string name)
		{
			FreePatternStore fps = null;
			
			// Check dict for store
			if (storesDict_.ContainsKey(name)) {
				fps = storesDict_[name];
			}
			
			// Otherwise attempt to load it dynamically
			if (fps == null) {
				fps = loadStore(name);
			}
			
			if (fps == null) {
				Debug.LogError("Failed to select FreePatternStore '"+name+"'.");
				return;
			}
			
			setTarget(fps);
		}
		
		
		private void newStore(string storeName)
		{
			// Check name isn't already taken in editor
			if (storesDict_.ContainsKey(storeName)) {
				EditorUtility.DisplayDialog("FreePatternStore Editor - Warning",
											"Can't create store because the name '"+storeName+"' it is already taken.",
											"Ok");
				return;
			}
			
			// Check store asset doesn't already exist
			string path = storeDirectory_ + storeName + ".txt";
			if (File.Exists(path)) {
				EditorUtility.DisplayDialog("FreePatternStore Editor - Warning",
											"Can't create store because the asset name '"+storeName+"' is already taken.",
											"Ok");
				return;
			}
			
			FreePatternStore fps = ScriptableObject.CreateInstance<FreePatternStore>();
			storesDict_[storeName] = fps;
			fileStates_[storeName] = FileState.unsaved;
			selectStore(storeName);
		}
		
		
		public string nameForStore(FreePatternStore fps)
		{
			return storesDict_.FirstOrDefault(s => s.Value == fps).Key;
		}
		
		
		#endregion Array Handling
		
		
		#region Loading and Saving
		
		
		public FreePatternStore loadStore(string name)
		{
			// Check it hasn't been loaded already
			if (storesDict_.ContainsKey(name)) {
				Debug.LogError("Attempted to load FreePatternStore '"+name+"' when it already exists");
				return storesDict_[name];
			}
			
			string path = storeDirectory_ + name + ".txt";
			
			if (!File.Exists(path)) {
				Debug.LogError("FreePatternStore with path '"+path+"' doesn't exist");
				return null;
			}
			
			TextAsset asset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
			return loadStore(asset);
		}
		
		
		public FreePatternStore loadStore(TextAsset asset)
		{
			string json = asset.text;
			FreePatternStore fps = ScriptableObject.CreateInstance<FreePatternStore>();
			Dictionary<string, object> data = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
			fps.unpackDict(data);
			
			string name = asset.name;
			storesDict_[name] = fps;
			fileStates_[name] = FileState.saved;
			
			return fps;
		}
		
		
		private void saveStore(string storeName)
		{
			// Check store exist in editor
			if (!storesDict_.ContainsKey(storeName)){
				Debug.LogError("Can't save store '"+storeName+"' because it doesn't exist in editor");
				return;
			}
			
			string path = storeDirectory_ + storeName + ".txt";
			
			// Warn of overwrite
			if (File.Exists(path)) {
				if (!EditorUtility.DisplayDialog("FreePatternStoreEditor - Warning",
												 "Continuing to save will overwrite '"+storeName+"'.",
												 "Save",
												 "Cancel")) {
					return;
				}
			}
			
			// Serialize store
			FreePatternStore store = storesDict_[storeName];
			string json = MiniJSON.Json.Serialize(store.packDict());
			
			// Add to assets and update state
			File.WriteAllText(path, json);
			AssetDatabase.Refresh();
			fileStates_[storeName] = FileState.saved;
		}
		
		
		#endregion Loading and Saving
		
		
		#region Store Manipulation
		
		
		private void addColumnRight()
		{
			if (serializedTarget_ == null) {
				Debug.LogError("Tried to add new column to FreePatternStore without selection.");
				return;
			}
			
			serializedTarget_.Update();
			
			// Values are flat packed so cells representing
			// the end of the row need to be inserted.
			for (int i=height_.intValue; i>0; i--) {
				values_.InsertArrayElementAtIndex(i * width_.intValue);
				values_.GetArrayElementAtIndex(i * width_.intValue).enumValueIndex = (int)CellType.Normal;
			}
			width_.intValue++;
			
			serializedTarget_.ApplyModifiedProperties();
		}
		
		
		private void addColumnLeft()
		{
			if (serializedTarget_ == null) {
				Debug.LogError("Tried to add new column to FreePatternStore without selection.");
				return;
			}
			
			serializedTarget_.Update();
			
			// Values are flat packed so cells representing
			// the start of the row need to be inserted.
			for (int i=height_.intValue; i>0; i--) {
				values_.InsertArrayElementAtIndex(i*width_.intValue);
				values_.GetArrayElementAtIndex(i * width_.intValue).enumValueIndex = (int)CellType.Normal;
			}
			width_.intValue++;
			
			serializedTarget_.ApplyModifiedProperties();
		}
		
		
		private void addRowTop()
		{
			if (serializedTarget_ == null) {
				Debug.LogError("Tried to add new row to FreePatternStore without selection.");
				return;
			}
			
			serializedTarget_.Update();
			
			// Append new row to end of array
			for (int i=0; i<width_.intValue; i++) {
				values_.InsertArrayElementAtIndex(width_.intValue*height_.intValue+i);
				values_.GetArrayElementAtIndex(width_.intValue*height_.intValue+i).enumValueIndex = (int)CellType.Normal;
			}
			height_.intValue++;
			
			serializedTarget_.ApplyModifiedProperties();
		}
		
		
		private void addRowBottom()
		{
			if (serializedTarget_ == null) {
				Debug.LogError("Tried to add new row to FreePatternStore without selection.");
				return;
			}
			
			serializedTarget_.Update();
			
			// Insert new row to start of array
			for (int i=0; i<width_.intValue; i++) {
				values_.InsertArrayElementAtIndex(i);
				values_.GetArrayElementAtIndex(i).enumValueIndex = (int)CellType.Normal;
			}
			height_.intValue++;
			
			serializedTarget_.ApplyModifiedProperties();
		}
		
		
		private void removeColumnRight()
		{
			if (serializedTarget_ == null) {
				Debug.LogError("Tried to remove row from FreePatternStore without selection.");
				return;
			}
			
			if (width_.intValue < 2) {
				Debug.LogError("FreePatternStore cannot have fewer than 1 column");
				return;
			}
			
			serializedTarget_.Update();
			
			for (int i=0; i<height_.intValue; i++) {
				for(int j = (width_.intValue - 1) * (i + 1); j<valuesCount_.intValue; j++) {
					values_.MoveArrayElement(j, j-1);
				}
				valuesCount_.intValue--; // this also shrinks the array
			}
			width_.intValue--;
			
			serializedTarget_.ApplyModifiedProperties();
		}
		
		
		private void removeColumnLeft()
		{
			if (serializedTarget_ == null) {
				Debug.LogError("Tried to remove row from FreePatternStore without selection.");
				return;
			}
			
			if (width_.intValue < 2) {
				Debug.LogError("FreePatternStore cannot have fewer than 1 column");
				return;
			}
			
			serializedTarget_.Update();
			
			for (int i=0; i<height_.intValue; i++) {
				for(int j = (width_.intValue - 1) * i; j<valuesCount_.intValue; j++) {
					values_.MoveArrayElement(j+1, j);
				}
				valuesCount_.intValue--; // this also shrinks the array
			}
			width_.intValue--;
			
			serializedTarget_.ApplyModifiedProperties();
		}
		
		
		private void removeRowTop()
		{
			if (serializedTarget_ == null) {
				Debug.LogError("Tried to remove row from FreePatternStore without selection.");
				return;
			}
			
			if (height_.intValue < 2) {
				Debug.LogError("FreePatternStore cannot have fewer than 1 row");
				return;
			}
			
			serializedTarget_.Update();
			
			valuesCount_.intValue -= width_.intValue;
			height_.intValue--;
			
			serializedTarget_.ApplyModifiedProperties();
		}
		
		
		private void setType(CellType type, int col, int row)
		{
			serializedTarget_.Update();
			values_.GetArrayElementAtIndex(row*width_.intValue+col).enumValueIndex = (int)type;
			serializedTarget_.ApplyModifiedProperties();
		}
		
		
		#endregion Store Manipulation
		
		
		#region Pattern View Methods
		
		
		public int numberOfColumns(PatternArrayView view)
		{
			if (serializedTarget_ == null)
				return 0;
			
			FreePatternStore store = serializedTarget_.targetObject as FreePatternStore;
			return store.width;
		}
		
		
		public int numberOfRows(PatternArrayView view)
		{
			if (serializedTarget_ == null)
				return 0;
			
			FreePatternStore store = serializedTarget_.targetObject as FreePatternStore;
			return store.height;
		}
		
		
		public CellType typeForCell(PatternArrayView view, int col, int row)
		{
			if (serializedTarget_ == null)
				return CellType.Undefined;
			
			FreePatternStore store = serializedTarget_.targetObject as FreePatternStore;
			
			// Check bounds
			if (   col >= 0 && col < store.width
				&& row >= 0 && row < store.height)
				return store[col, row];
			else
				return CellType.Undefined;
		}
		
		
		public bool firstColumnIsEven(PatternArrayView view)
		{
			return true;
		}
		
		
		public void cellLeftClicked(PatternArrayView view, int col, int row)
		{
			setType(cellTypes_[leftClickCellTypeIndex_], col, row);
		}
		
		public void cellRightClicked(PatternArrayView view, int col, int row)
		{
			setType(cellTypes_[rightClickCellTypeIndex_], col, row);
		}
		
		
		#endregion Pattern View Methods
	}

}

