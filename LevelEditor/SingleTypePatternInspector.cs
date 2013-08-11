using System;
using UnityEditor;
using Penguin;

namespace LevelEditor
{
	public class SingleTypePatternInspector : CellPatternInspector
	{	
		
		public override void OnGUI ()
		{
			base.OnGUI();
			
			EditorGUILayout.LabelField("moo!");
		}
	}
}

