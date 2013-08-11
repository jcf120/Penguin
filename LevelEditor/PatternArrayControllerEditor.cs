using System;
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
		
		
		// Serialized data
		private SerializedObject   controller_;
		private SerializedProperty title_;
		private SerializedProperty patternCount_;
		
		
		// Serialized property paths
		private static string patternSizePath_ = "patterns.Array.size";
		private static string patternDataPath_ = "patterns.Array.data[{0}]";
		
		
		// Interface data
		private string newPatType_ = "SingleType";
		private PatternArrayController controllerToLoad_;
		
		
		// Instead of instantiation
		public void OnEnable ()
		{
			// Create new level if none selected
			if (controller_ == null) {
				PatternArrayController pac = ScriptableObject.CreateInstance<PatternArrayController>();
				setController(pac);
			}
		}
		
		
		public  void OnGUI ()
		{
			// Asset loading
			EditorGUILayout.BeginHorizontal();
			controllerToLoad_ = EditorGUILayout.ObjectField(controllerToLoad_, typeof(PatternArrayController), true) as PatternArrayController;
			var oldEnabled = GUI.enabled; // state-machine housekeeping
			GUI.enabled = controllerToLoad_!=null ? true : false;
			if (GUILayout.Button("Load"))
				setController(controllerToLoad_);
			GUI.enabled = oldEnabled;
			EditorGUILayout.EndHorizontal();
			
			controller_.Update();
			
			title_.stringValue = EditorGUILayout.TextField("Title", title_.stringValue);
			
			if (canSave() && GUILayout.Button("Save"))
					save();
			
			// Temp pattern type input
			newPatType_ = EditorGUILayout.TextField(newPatType_);
			
			if (GUILayout.Button("New Pattern"))
				newPattern();
			
			controller_.ApplyModifiedProperties();
		}
		
		
		private void setController(PatternArrayController pac)
		{
			controller_ = new SerializedObject(pac);
				
			patternCount_ = controller_.FindProperty(patternSizePath_);
			title_ = controller_.FindProperty("title");
		}
		
		
		private bool canSave()
		{
			if (controller_ == null)
				return false;
			
			return true;
		}
		
		
		private void save()
		{
			string json = MiniJSON.Json.Serialize(controllerToDict());
			Debug.Log (json);
		}
		
		
		private Dictionary<string,object> controllerToDict()
		{
			Dictionary<string,object> data = new Dictionary<string, object>();
			
			data["title"] = title_.stringValue;
			
			CellPattern[] patterns = patternsArray();
			ArrayList patsData = new ArrayList();
			foreach (CellPattern pat in patterns) {
				patsData.Add(pat.packDict());
			}
			data["patterns"] = patsData;
			
			return data;
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
			//CellPattern pattern = (CellPattern)ScriptableObject.CreateInstance(newPatType+"Pattern");
			CellPattern pattern = (CellPattern)ScriptableObject.CreateInstance<SingleTypePattern>();
			if (pattern == null) {
				Debug.LogError("CellPattern subclass '"+newPatType_+"' doesn't exist");
				return;
			}

			// Give default size
			pattern.colsLeft  = 5;
			pattern.colsRight = 5;
			pattern.rows      = 5;
			
			// Temp set cell type
			((SingleTypePattern)pattern).cellType = CellType.Normal;

			// Calculate offset (sum vertical size)
			PatternCoordinate offset = PatternCoordinate.zero;
			CellPattern[] patterns = patternsArray();
			foreach (CellPattern cp in patterns) {
				Debug.Log(cp);
			}
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
			Debug.Log (patternCount_.intValue);
		}
		
	}
}

