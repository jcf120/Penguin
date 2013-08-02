using System;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	public class LevelController
	{
		
		private Level level_;
		
		public LevelController ()
		{
			level_ = new Level();
		}
		
		
		public void newPattern(string typeStr, PatternCoordinate origin)
		{
			// Check class exists for string
			Type patType = Type.GetType(typeStr);
			if (patType == null) {
				Debug.LogError("No CellPattern subclass: "+typeStr);
				return;
			}
			
			CellPattern pat = (CellPattern)Activator.CreateInstance(patType);
			pat.origin = origin;
			
			// Set default size
			pat.rows      = 10;
			pat.colsLeft  = 5;
			pat.colsRight = 5;
			
			level_.addPattern(pat);
		}
	}
}

