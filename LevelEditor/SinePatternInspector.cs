using System;
using UnityEditor;

namespace LevelEditor
{
	public class SinePatternInspector : CellPatternInspector
	{
		
		private SerializedProperty amplitude_;
		private SerializedProperty wavelength_;
		private SerializedProperty width_;
		
		
		public override void setTarget (Penguin.CellPattern pattern)
		{
			base.setTarget (pattern);
			
			amplitude_  = serializedTarget_.FindProperty("amplitude" );
			wavelength_ = serializedTarget_.FindProperty("wavelength");
			width_      = serializedTarget_.FindProperty("width"     );
		}
		
		
		protected override void propertiesGUI ()
		{
			base.propertiesGUI ();
			
			EditorGUILayout.PropertyField(amplitude_ );
			EditorGUILayout.PropertyField(wavelength_);
			EditorGUILayout.PropertyField(width_     );
			if (width_.intValue < 1)
				width_.intValue = 1;
		}
	}
}

