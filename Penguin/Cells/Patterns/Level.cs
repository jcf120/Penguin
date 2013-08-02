using System;
using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
	public class Level
	{
		
		private List<CellPattern> patterns_;
		
		public Level ()
		{
			patterns_ = new List<CellPattern>();
		}
		
		
		public void addPattern(CellPattern pattern)
		{
			// Sort into list based row depth
			patterns_.Add(pattern);
			patterns_.Sort((p1, p2) => {
				if 		(p1.origin.row == p2.origin.row)
					return  0;
				else if (p1.origin.row >  p2.origin.row)
					return  1;
				else
					return -1;
			});
		}
		
		
		public void removePattern(CellPattern pattern)
		{
			patterns_.Remove(pattern);
		}
		
		
		public CellPattern patternAtCoor(PatternCoordinate coor)
		{	
			// Check vertical region
			// This search could be made more efficient, it needn't iterate over all items
			
			CellPattern[] pats =  patterns_.FindAll((pat) => {
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

