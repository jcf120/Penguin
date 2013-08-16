using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	
	public class FreePatternStoreEditor
	{
		
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
		
		
		// New store GUI
		private string newStoreName_ = "untitled";
		
		
		// Save/load directory
		private static string storeDirectory_ = "Assets/Levels/FreePatterns/";
		
		
		public FreePatternStoreEditor()
		{
			storesDict_ = new Dictionary<string, FreePatternStore>();
			fileStates_ = new Dictionary<string, FileState>();
		}
		
		
		public void OnGUI ()
		{
			newStoreGUI();
			
			if (serializedTarget_ == null)
				return;
			
			saveStoreGUI();
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
		
		
		private void saveStoreGUI()
		{
			EditorGUILayout.BeginHorizontal();
			
			// Look selected store's name
			string storeName = nameForStore((FreePatternStore)serializedTarget_.targetObject);
			// Append file state
			string storeLabel = storeName + " - " + fileStates_[storeName].ToString();
			EditorGUILayout.LabelField(storeLabel);
			if (GUILayout.Button("Save"))
				saveStore(storeName);
			
			EditorGUILayout.EndHorizontal();
		}
		
		
		private void setTarget(FreePatternStore store)
		{
			serializedTarget_ = new SerializedObject(store);
			
			width_  = serializedTarget_.FindProperty("width" );
			height_ = serializedTarget_.FindProperty("height");
			values_ = serializedTarget_.FindProperty("values");
		}
		
		
		public string nameForStore(FreePatternStore fps)
		{
			return storesDict_.FirstOrDefault(s => s.Value == fps).Key;
		}
		
		
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
			string json = asset.text;
			FreePatternStore fps = ScriptableObject.CreateInstance<FreePatternStore>();
			Dictionary<string, object> data = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
			fps.unpackDict(data);
			
			storesDict_[name] = fps;
			fileStates_[name] = FileState.saved;
			
			return fps;
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
	}

}
