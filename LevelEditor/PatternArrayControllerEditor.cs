using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	public class PatternEditor : EditorWindow
	{
		
		private SerializedObject  controller_;
		private List<CellPattern> patterns_;
		
		
		private string newPatType = "SingleType";
		
		public void OnEnable ()
		{
			// Create new level if none selected
			if (controller_ == null) {
				ScriptableObjectUtility.CreateAsset<PatternArrayController>();
				PatternArrayController pac = ScriptableObject.CreateInstance<PatternArrayController>();
				controller_ = new SerializedObject(pac);
			
				patterns_ = pac.patterns;
			}
		}
		
		
		[MenuItem("Window/Pattern Editor")]
		public static void ShowWindow ()
		{
			PatternEditor pe = EditorWindow.GetWindow<PatternEditor>("Levels/Lvl1");
			pe.title = "Pattern Editor";
		}
		
		
		public  void OnGUI ()
		{
			// Asset loading
			/*EditorGUILayout.ObjectField(null, typeof(AssetsItem), false);
			
			controller_.Update();
			
			// Temp pattern type input
			newPatType = EditorGUILayout.TextField(newPatType);
			
			if (GUILayout.Button("New Pattern"))
				newPattern();
			
			controller_.ApplyModifiedProperties();*/
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
			foreach (CellPattern cp in patterns_) {
				Debug.Log(cp);
			}
			foreach (CellPattern cp in patterns_) {
				offset.row += cp.rows;
			}
			// col offset inherited from current end pattern
			// Check isn't empty first
			if (patterns_.Count > 0)
				offset.col = patterns_.Last().origin.col;
			else
				offset.col = 0;
			
			// Apply and append
			pattern.origin = offset;
			patterns_.Add(pattern);
			Debug.Log (patterns_.Count);
		}
		
	}
}

