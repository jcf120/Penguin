using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	public class PatternEditor : EditorWindow
	{
		[MenuItem("Window/Pattern Editor")]
		public static void ShowWindow ()
		{
			var window = GetWindow<PatternEditor>();
			window.title = "Pattern Editor";
		}
		
		private string title_ = "untitled";
		
		// Serialized data
		private SerializedObject   controller_;
		private SerializedProperty patternCount_;
		
		
		// Serialized property paths
		private static string patternSizePath_ = "patterns.Array.size";
		private static string patternDataPath_ = "patterns.Array.data[{0}]";
		
		
		// Interface data
		
		// Loading GUI
		private TextAsset assetToLoad_;
		// List GUI
		private Vector2 patListScrollPos_;
		private int     selectedPatIndex_ = -1;
		// New Pattern GUI
		string[] patternClassNames_;
		int      selectedClass_ = 0;
		// Pattern Inspector GUI
		private bool                 isInspectorVisible_ = true;
		private CellPatternInspector patInspector_;
		// FreePatternStore Inspector GUI
		private FreePatternStoreEditor storeEditor_;
		
		
		// Instead of instantiation
		public void OnEnable ()
		{
			// Create new level if none selected
			if (controller_ == null) {
				PatternArrayController pac = ScriptableObject.CreateInstance<PatternArrayController>();
				setController(pac);
			}
			
			// Populate CellPattern subclass list
			var subclasses = typeof(CellPattern).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(CellPattern)));
			patternClassNames_ = (from sc in subclasses select sc.Name).ToArray();
			
			storeEditor_ = new FreePatternStoreEditor();
		}
		
		
		private void loadStoresForPacDict(Dictionary<string, object> pacDict)
		{
			// Determine required patterns
			List<string> requiredStores = new List<string>();
			List<object> patDicts = pacDict["patterns"] as List<object>;
			foreach (Dictionary<string, object> patDict in patDicts) {
				
				// Is it a free pattern?
				if (patDict["class"].ToString() == typeof(FreePattern).ToString()) {
					
					string storeName = patDict["storeName"] as string;
					
					// Add if it hasn't been already
					if (!requiredStores.Contains(storeName))
						requiredStores.Add(storeName);
				}
			}
			
			// Request each store from storeEditor unless labelled 'unassigned'
			foreach (string storeName in requiredStores) {
				if (storeName != "unassigned")
					storeEditor_.loadStore(storeName);
			}
		}
		
		
		public  void OnGUI ()
		{
			loadingGUI();
			
			if (controller_==null)
				return;
			
			controller_.Update();
			
			savingGUI();
			tableGUI();
			newPatternGUI();
			patternInspectorGUI();
			storeEditor_.OnGUI();
			
			controller_.ApplyModifiedProperties();
		}
		
		
		private void loadingGUI()
		{
			EditorGUILayout.BeginHorizontal();
			assetToLoad_ = EditorGUILayout.ObjectField(assetToLoad_, typeof(TextAsset), true) as TextAsset;
			bool oldEnabled = GUI.enabled; // state-machine housekeeping
			GUI.enabled = (assetToLoad_!=null) && oldEnabled;
			if (GUILayout.Button("Load"))
				load ();
			GUI.enabled = oldEnabled;
			EditorGUILayout.EndHorizontal();
		}
		
		
		private void savingGUI()
		{
			EditorGUILayout.BeginHorizontal();
			title_ = EditorGUILayout.TextField(title_);
			
			if (canSave() && GUILayout.Button("Save"))
					save();
			EditorGUILayout.EndHorizontal();
		}
		
		
		private void tableGUI()
		{
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Patterns List");
			patListScrollPos_ = EditorGUILayout.BeginScrollView(patListScrollPos_);
			EditorGUILayout.BeginVertical();
			
			// pull labels from patterns
			CellPattern[] patterns = patternsArray();
			string[] labels = new string[patterns.Length];
			for (int i=0; i<patterns.Length; i++) {
				labels[i] = patterns[i].GetType().Name;
			}
			// Display as selection table
			int newSel = GUILayout.SelectionGrid(selectedPatIndex_,labels,1);
			// Detect selection change
			if (newSel != selectedPatIndex_)
				selectPattern(newSel);
			
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}
		
		
		private void newPatternGUI()
		{
			EditorGUILayout.BeginHorizontal();
			selectedClass_ = EditorGUILayout.Popup(selectedClass_,patternClassNames_);
			
			if (GUILayout.Button("Add"))
				newPattern();
			
			EditorGUILayout.EndHorizontal();
		}
		
		
		private void patternInspectorGUI()
		{
			CellPattern pat = null;
			// Check for selection bounds then find pattern
			if (selectedPatIndex_>=0)
				pat = patternsArray ()[selectedPatIndex_];
			
			string label = "Pattern Inspector - ";
			label += pat!=null ? pat.GetType().Name : "no selection"; 
			
			isInspectorVisible_ = EditorGUILayout.Foldout(isInspectorVisible_,label);
			if (isInspectorVisible_ && patInspector_ != null)
				patInspector_.OnGUI();
		}
		
		
		private void selectPattern(int index)
		{
			// Negative index means no selection
			if (index < 0) {
				selectedPatIndex_ = -1;
				return;
			}
			// Check bounds
			if (index >= patternCount_.intValue) {
				Debug.LogError("can't select pattern beyond bounds: "+index);
				return;
			}
			
			// successful selection change
			selectedPatIndex_ = index;
			
			// Find inspector for pattern within editor namespace
			CellPattern pat = patternsArray ()[index];
			// Does inspector need instantiating or changing?
			if (patInspector_==null || !patInspector_.doesInspectPattern(pat.GetType()))
			{
				// Yes - so instantiate new corresponding class
				string classStr = "LevelEditor."+pat.GetType().Name+"Inspector";
				Type inspType = Type.GetType(classStr);
				if (inspType == null) {
					Debug.LogError("No inspector of class '"+classStr+"', deafaulting to CellPatternInspector.");
					inspType = typeof(CellPatternInspector);
				}
				patInspector_ = Activator.CreateInstance(inspType) as CellPatternInspector;
				
				// If inspecting a FreePattern, we need to pass in the stores dictionary
				if (patInspector_.GetType() == typeof(FreePatternInspector)) {
					FreePatternInspector fpInsp = patInspector_ as FreePatternInspector;
					fpInsp.storesDict = storeEditor_.storesDict;
				}
			}
			
			patInspector_.setTarget(pat);
		}
		
			
		private void setController(PatternArrayController pac)
		{
			controller_ = new SerializedObject(pac);
				
			patternCount_ = controller_.FindProperty(patternSizePath_);
		}
		
		
		private bool canSave()
		{
			if (controller_ == null)
				return false;
			
			return true;
		}
		
		
		private void save()
		{
			// Check if file already exists
			string path = "Assets/Levels/" + title_ + ".txt";
			if (File.Exists(path)) {
				if (!EditorUtility.DisplayDialog("Pattern Editor - Warning",
												 "Are you sure you want to save over "+title_+"?",
												 "Save",
												 "Cancel")) {
					// Cancel save
					return;
				}
			}
			
			// Convert structure to json
			PatternArrayController pac = (PatternArrayController)controller_.targetObject;
			string json = MiniJSON.Json.Serialize(pac.packDict());
			
			// Create asset
			File.WriteAllText(path, json);
			AssetDatabase.Refresh();
		}
		
		
		private void load()
		{
			// Warn of data loss
			if (!EditorUtility.DisplayDialog("Pattern Editor - Warning",
											 "Continuing to load will scrap the current level",
											 "Load",
											 "Cancel")) {
				// Cancel load
				return;
			}
			
			// Extract dictionary from json and unpack
			PatternArrayController pac = ScriptableObject.CreateInstance<PatternArrayController>() as PatternArrayController;
			string json = assetToLoad_.text;
			Dictionary<string,object> data = MiniJSON.Json.Deserialize(json) as Dictionary<string,object>;
			
			// Check json represented correct class
			if ((string)data["class"] != typeof(PatternArrayController).ToString()) {
				Debug.LogError("Can't unpack deserialized json into PatternArrayController that represents a '"+data["class"]+"'.");
			}
			
			loadStoresForPacDict(data);
			pac.unpackDict(data);
			FreePatternStoreLinker.linkPacToStores(pac, storeEditor_.storesDict);
			
			// Assign to editor
			setController(pac);
			
			// Clear interface selection
			selectPattern(-1);
		}


		private CellPattern[] patternsArray()
		{
			var count = controller_.FindProperty("patterns.Array.size").intValue;
			var array = new CellPattern[count];

			for (var i=0; i<count; i++) {
				array[i] = controller_.FindProperty(string.Format(patternDataPath_,i)).objectReferenceValue as CellPattern;
			}

			return array;
		}


		private void setPattern(int index,CellPattern pattern)
		{
			// Check array bounds, and allow growth by one element if necessary
			if      (index > patternCount_.intValue) {
				Debug.LogError("Cannot set pattern more than element beyond bounds (index:"+index+")");
				return;
			}
			else if (index == patternCount_.intValue) {
				patternCount_.intValue++;
			}
			controller_.FindProperty(string.Format(patternDataPath_,index)).objectReferenceValue = pattern;
		}


		private void newPattern()
		{
			// Build new CellPattern as from ScriptableObject
			string newPatType = patternClassNames_[selectedClass_];
			CellPattern pattern = (CellPattern)ScriptableObject.CreateInstance(newPatType);
			if (pattern == null) {
				Debug.LogError("CellPattern subclass '"+newPatType+"' doesn't exist");
				return;
			}

			// Calculate offset (sum vertical size)
			PatternCoordinate offset = PatternCoordinate.zero;
			CellPattern[] patterns = patternsArray();
			foreach (CellPattern cp in patterns) {
				offset.row += cp.rows;
			}
			// col offset inherited from current end pattern
			// Check isn't empty first
			if (patterns.Length > 0)
				offset.col = patterns[patterns.Length-1].origin.col;
			else
				offset.col = 0;

			// Apply and append
			pattern.origin = offset;
			setPattern(patternCount_.intValue, pattern);
		}
		
	}
}

