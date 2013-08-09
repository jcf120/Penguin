using System;
using UnityEditor;
using Penguin;

namespace LevelEditor
{
	[CustomEditor (typeof (PatternArrayController))]
	public class PatternArrayControllerEditor : Editor
	{
		
		private SerializedObject patternArrayController_;
		
		void OnEnable ()
		{
			patternArrayController_ = new SerializedObject(target);
		}
		
		
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
		}
		
	}
}

