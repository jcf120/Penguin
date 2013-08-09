using System;
using UnityEditor;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	[CustomEditor (typeof (PatternArrayController))]
	public class PatternArrayControllerEditor : Editor
	{
		
		private SerializedObject   controller_;
		private SerializedProperty patternCount_;
		
		
		private string newPatType = "SingleType";
		
		
		
		private static string patternSizePath_ = "patterns.Array.size";
		private static string patternDataPath_ = "patterns.Array.data[{0}]";
		
		public void OnEnable ()
		{
			hideFlags = HideFlags.DontSave;
			controller_ = new SerializedObject(target);
			patternCount_ = controller_.FindProperty(patternSizePath_);
		}
		
		
		public override void OnInspectorGUI ()
		{
			controller_.Update();
			
			// Temp pattern type input
			newPatType = EditorGUILayout.TextField(newPatType);
			
			if (GUILayout.Button("New Pattern"))
				newPattern();
			
			controller_.ApplyModifiedProperties();
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
				Debug.LogError("CellPattern subclass '"+newPatType+"' doesn't exist");
				return;
			}
			
			// Give default size
			pattern.colsLeft  = 5;
			pattern.colsRight = 5;
			pattern.rows      = 5;
			
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

