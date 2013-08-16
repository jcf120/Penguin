using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	public class FreePatternInspector : CellPatternInspector
	{
		private SerializedProperty storeName_;
		private SerializedProperty store_;
		
		public Dictionary<string, FreePatternStore> storesDict;
		
		public override void setTarget (Penguin.CellPattern pattern)
		{
			base.setTarget (pattern);
			storeName_ = serializedTarget_.FindProperty("storeName");
			store_     = serializedTarget_.FindProperty("store");
		}
		
		
		protected override void propertiesGUI ()
		{
			base.propertiesGUI ();
			
			if (storesDict == null) {
				Debug.LogError("FreePatternInspector hasn't been assigned a storesDict.");
				return;
			}
			
			// Convert dictionary to array of names and add null selection
			List<string> storeNames = (from kvp in storesDict select kvp.Key).ToList();
			storeNames.Add ("unassigned");
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Store");
			// Display as popup
			int selIndex = storeNames.IndexOf(storeName_.stringValue);
			selIndex = EditorGUILayout.Popup(selIndex, storeNames.ToArray());
			
			EditorGUILayout.EndHorizontal();
			
			// Update properties
			storeName_.stringValue = storeNames[selIndex];
			if (storeName_.stringValue == "unassigned") {
				store_.objectReferenceValue = null;
			}
			else {
				store_.objectReferenceValue = storesDict[storeName_.stringValue];
			}
			
		}
	}
}
