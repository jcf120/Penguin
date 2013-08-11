using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	public class CellPatternInspector
	{
		protected SerializedObject serializedTarget_;
		
		private SerializedProperty originCol_;
		private SerializedProperty originRow_;
		private SerializedProperty rows_;
		private SerializedProperty colsLeft_;
		private SerializedProperty colsRight_;
		
		
		public virtual void setTarget(CellPattern pattern)
		{
			if (!doesInspectPattern(pattern.GetType())) {
				Debug.LogError("Inspector can't inspect type '"+pattern.GetType().ToString()+"'.");
				return;
			}
			
			serializedTarget_ = new SerializedObject(pattern);
			
			originCol_ = serializedTarget_.FindProperty("originCol");
			originRow_ = serializedTarget_.FindProperty("originRow");
			rows_      = serializedTarget_.FindProperty("rows"     );
			colsLeft_  = serializedTarget_.FindProperty("colsLeft" );
			colsRight_ = serializedTarget_.FindProperty("colsRight");
		}
		
		
		public bool doesInspectPattern(Type patternType)
		{
			// Find corresponding class string
			string classStr = GetType().Namespace + "." + patternType.Name + "Inspector";
			return GetType().ToString() == classStr;
		}
		
		
		public void OnGUI()
		{
			if (serializedTarget_ == null) {
				EditorGUILayout.LabelField("No target");
				return;
			}
			
			serializedTarget_.Update();
			propertiesGUI();
			serializedTarget_.ApplyModifiedProperties();
		}
		
		
		protected virtual void propertiesGUI ()
		{
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Origin");
			EditorGUILayout.PropertyField(originCol_,GUIContent.none);
			// Origin column must be even
			if (originCol_.intValue % 2 != 0)
				originCol_.intValue++;
			EditorGUILayout.PropertyField(originRow_,GUIContent.none);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.PropertyField(rows_     );
			EditorGUILayout.PropertyField(colsLeft_ );
			EditorGUILayout.PropertyField(colsRight_);
		}
	}
}

