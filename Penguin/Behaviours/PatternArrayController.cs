using System;
using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
	[Serializable]
	public class PatternArrayController : ScriptableObject
	{
		
		public string title = "untitled";
		public List<CellPattern> patterns;
		
		
		public void OnEnable ()
		{
			if (patterns == null)
				patterns = new List<CellPattern>();
			
			hideFlags = HideFlags.HideInInspector;
		}
		
		
		public void removePattern(CellPattern pattern)
		{
			//patterns.Remove(pattern);
		}
		
		
		public CellPattern patternAtCoor(PatternCoordinate coor)
		{	
			// Check vertical region
			// This search could be made more efficient, it needn't iterate over all items
			
			/*CellPattern[] pats =  patterns.FindAll((pat) => {
				return (pat.origin.row <= coor.row) && (pat.origin.row + pat.rows > coor.row);
			}).ToArray();
			
			// Check horizontal region
			// return first matching pattern
			foreach (CellPattern pat in pats)
			{
				if (   pat.origin.col - pat.colsLeft  <= coor.col 
					&& pat.origin.col + pat.colsRight >= coor.col) {
					return pat;
				}
			}*/
			
			// No match found
			return null;
		}
		
		
		public CellType typeAtCoor(PatternCoordinate coor)
		{
			CellPattern pat = patternAtCoor(coor);
			
			// If no pattern present, return empty
			if (pat == null)
				return CellType.Empty;
			
			// Calc relative coordinates
			coor.row -= pat.origin.row;
			coor.col -= pat.origin.col;
			
			return pat.typeAtCoordinate(coor);
		}
	}
}

