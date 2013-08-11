using System;
using UnityEditor;
using Penguin;

namespace LevelEditor
{
	public class SingleTypePatternInspector : CellPatternInspector
	{	
		
		private SerializedProperty cellType_;
		
		public override void setTarget (CellPattern pattern)
		{
			base.setTarget (pattern);
			
			cellType_ = serializedTarget_.FindProperty("cellType");
		}
		
		
		protected override void propertiesGUI()
		{
			base.propertiesGUI();
			
			EditorGUILayout.PropertyField(cellType_);
		}
	}
}

