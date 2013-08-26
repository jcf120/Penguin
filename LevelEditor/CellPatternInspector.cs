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
		
		protected SerializedProperty originCol_;
		protected SerializedProperty originRow_;
		protected SerializedProperty rows_;
		protected SerializedProperty colsLeft_;
		protected SerializedProperty colsRight_;
		
		
		public virtual void setTarget(CellPattern pattern)
		{
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
			
			
			EditorGUILayout.PropertyField(originCol_);
			// Origin column must be even
			if (originCol_.intValue % 2 != 0)
				originCol_.intValue++;
			//EditorGUILayout.PropertyField(originRow_); // Shouldn't be directly editable
			EditorGUILayout.LabelField("Origin Row",originRow_.intValue.ToString());
			EditorGUILayout.PropertyField(rows_     );
			EditorGUILayout.PropertyField(colsLeft_ );
			EditorGUILayout.PropertyField(colsRight_);
		}
	}
}

