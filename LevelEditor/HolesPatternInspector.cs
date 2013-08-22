using System;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
	public class HolesPatternInspector : CellPatternInspector
	{
		private SerializedProperty size_;
		private SerializedProperty separation_;
		
		
		public override void setTarget (Penguin.CellPattern pattern)
		{
			base.setTarget (pattern);
			size_       = serializedTarget_.FindProperty("size");
			separation_ = serializedTarget_.FindProperty("separation");
		}
		
		
		protected override void propertiesGUI ()
		{
			base.propertiesGUI ();
			EditorGUILayout.PropertyField(size_      );
			EditorGUILayout.PropertyField(separation_);
		}
	}
}

