using System;
using UnityEngine;
using Penguin;

namespace LevelEditor
{
	public class LevelController
	{
		
		private Level level_;
		public  Level level{get{return level_;}}
		
		public LevelController ()
		{
			level_ = new Level();
			CellPattern cp = new HorizontalBreaksPattern(2,2);
			cp.colsLeft  = 10;
			cp.colsRight = 10;
			cp.rows      = 20;
			cp.origin    = new PatternCoordinate(0, 0);
			level_.addPattern(cp);
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

