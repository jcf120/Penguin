using System;
using UnityEditor;
using Penguin;

namespace LevelEditor
{
	[CustomEditor (typeof (LevelController))]
	public class LevelControllerEditor : Editor {
	
		private SerializedObject level_;
		private SerializedProperty name_;
		
		public void OnEnable()
		{
			level_ = new SerializedObject(target);
			name_  = level_.FindProperty("title2");
		}
		
		public override void OnInspectorGUI ()
		{
			level_.Update();
			
			EditorGUILayout.PropertyField(name_);
			level_.ApplyModifiedProperties();
		}
		
	}
	
}

