using System;
using UnityEditor;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	[CustomEditor (typeof (PatternArrayController))]
	public class PatternArrayControllerEditor : Editor
	{
		
		private SerializedObject   patternArrayController_;
		
		
		private string newPatType = "Normal";
		
		void OnEnable ()
		{
			patternArrayController_ = new SerializedObject(target);
		}
		
		
		public override void OnInspectorGUI ()
		{
			patternArrayController_.Update();
			
			// Temp pattern type input
			newPatType = EditorGUILayout.TextField(newPatType);
			
			if (GUILayout.Button("New Pattern"))
				newPattern();
		}
		
		
		private static string arraySizeStr = "patterns.Array.size";
		private static string arrayDataStr = "patterns.Array.data[{0}]";
		
		
		private CellPattern[] patternsArray()
		{
			var count = patternArrayController_.FindProperty(arraySizeStr).intValue;
			var array = new CellPattern[count];
			
			for (var i=0; i<count; i++) {
				array[i] = patternArrayController_.FindProperty(string.Format(arrayDataStr,i)).objectReferenceValue as CellPattern;
			}
			
			return array;
		}
		
		
		private void setPattern(int index,CellPattern pattern)
		{
			patternArrayController_.FindProperty(string.Format(arrayDataStr, index)).objectReferenceValue = pattern;
		}
		
		
		private void newPattern()
		{
			// Build new CellPattern as from ScriptableObject
			CellPattern pattern = (CellPattern)ScriptableObject.CreateInstance(newPatType);
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
			setPattern(patterns.Length, pattern);
		}
		
	}
}

