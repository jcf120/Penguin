using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
	
	public class PatternArrayController: MonoBehaviour
	{
		
		[SerializeField]
		private List<CellPattern> patterns;
		
		public PatternArrayController ()
		{
			patterns = new List<CellPattern>();
		}
		
		
		// Needs changing - CellPatterns inherit from ScriptableObject, so should be instantiated here
		public void newPattern(string patternTypeStr)
		{
			// Build new CellPattern as from ScriptableObject
			CellPattern pattern = (CellPattern)ScriptableObject.CreateInstance(patternTypeStr);
			if (pattern == null) {
				Debug.LogError("CellPattern subclass '"+patternTypeStr+"' doesn't exist");
				return;
			}
			
			// Give default size
			pattern.colsLeft  = 5;
			pattern.colsRight = 5;
			pattern.rows      = 5;
			
			// Calculate offset (sum vertical size)
			PatternCoordinate offset = PatternCoordinate.zero;
			foreach (CellPattern cp in patterns) {
				offset.row += cp.rows;
			}
			// col offset inherited from current end pattern
			offset.col = patterns.Last().origin.col;
			// Apply and append
			pattern.origin = offset;
			patterns.Add(pattern);
			
			
			// Sort into list based row depth
			/*patterns.Add(pattern);
			patterns.Sort((p1, p2) => {
				if 		(p1.origin.row == p2.origin.row)
					return  0;
				else if (p1.origin.row >  p2.origin.row)
					return  1;
				else
					return -1;
			});*/
		}
		
		
		public void removePattern(CellPattern pattern)
		{
			patterns.Remove(pattern);
		}
		
		
		public CellPattern patternAtCoor(PatternCoordinate coor)
		{	
			// Check vertical region
			// This search could be made more efficient, it needn't iterate over all items
			
			CellPattern[] pats =  patterns.FindAll((pat) => {
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
			}
			
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

