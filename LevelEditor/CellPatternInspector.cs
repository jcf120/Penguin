using System;
using UnityEditor;
using Penguin;

namespace LevelEditor
{
	public class CellPatternInspector
	{
		public CellPattern target;
		
		public virtual void OnGUI ()
		{
			EditorGUILayout.LabelField("Hello!");
		}
	}
}

