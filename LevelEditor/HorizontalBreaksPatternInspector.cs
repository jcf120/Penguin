using System;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
	public class HorizontalBreaksPatternInspector : CellPatternInspector
	{
		private SerializedProperty breakSize_;
		private SerializedProperty intervalSize_;
		
		public override void setTarget (Penguin.CellPattern pattern)
		{
			base.setTarget (pattern);
			breakSize_    = serializedTarget_.FindProperty("breakSize"   );
			intervalSize_ = serializedTarget_.FindProperty("intervalSize");
		}
		
		
		protected override void propertiesGUI ()
		{
			base.propertiesGUI ();
			EditorGUILayout.PropertyField(breakSize_   );
			EditorGUILayout.PropertyField(intervalSize_);
		}
	}
}

