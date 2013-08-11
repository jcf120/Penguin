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
		
		
		public void setTarget(CellPattern pattern)
		{
			if (!doesInspectPattern(pattern.GetType())) {
				Debug.LogError("Inspector can't inspect type '"+pattern.GetType().ToString()+"'.");
				return;
			}
			
			serializedTarget_ = new SerializedObject(pattern);
		}
		
		
		public bool doesInspectPattern(Type patternType)
		{
			// Find corresponding class string
			string classStr = GetType().Namespace + "." + patternType.Name + "Inspector";
			return GetType().ToString() == classStr;
		}
		
		
		public virtual void OnGUI ()
		{
			if (serializedTarget_ == null) {
				EditorGUILayout.LabelField("No target");
				return;
			}
				
			EditorGUILayout.LabelField("Hello!");
		}
	}
}

