using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
	public class PatternArrayController : ScriptableObject
	{
		
		public List<CellPattern> patterns;
		public Dictionary<string,FreePatternStore> freePatsDict;
		
		
		public Dictionary<string, object> packDict ()
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			
			data["class"] = GetType().ToString();
			
			ArrayList patsData = new ArrayList();
			foreach (CellPattern pat in patterns) {
				patsData.Add(pat.packDict());
			}
			data["patterns"] = patsData;
			
			return data;
		}
		
		
		public void unpackDict (Dictionary<string, object> data)
		{
			
			// Check pattern list hasn't already been filled
			if (patterns.Count > 0) {
				Debug.LogError("PatternArrayController unpacking failed - patterns List already filled.");
				return;
			}
			
			List<object> patDatas = data["patterns"] as List<object>;
			foreach (Dictionary<string, object> patData in patDatas) {
				
				// Determine CellPattern subclass and instantiate
				string typeStr = (string)patData["class"];
				if (Type.GetType(typeStr) == null) {
					Debug.LogError("Can't unpack CellPattern subclass '" + typeStr + "' because it doesn't exist. Defaulting to SingleTypePattern");
					typeStr = typeof(SingleTypePattern).ToString();
					patData["cellType"] = Enum.GetName(typeof(CellType),CellType.Normal); // prevent attempted unpacking of nonexistant data
				}
				
				CellPattern pat = ScriptableObject.CreateInstance(Type.GetType(typeStr)) as CellPattern;
				pat.unpackDict(patData);
				patterns.Add(pat);
			}
				
		}
		
		
		public void OnEnable ()
		{
			if (patterns == null)
				patterns = new List<CellPattern>();
			
			if (freePatsDict == null)
				freePatsDict = new Dictionary<string, FreePatternStore>();
			
			hideFlags = HideFlags.HideInInspector;
		}
		
		
		public CellPattern patternAtCoor(PatternCoordinate coor)
		{	
			// Check vertical region
			// This search could be made more efficient, it needn't iterate over all items
			
			CellPattern[] pats =  patterns.FindAll((pat) => {
				return (pat.origin.row <= coor.row) && (pat.origin.row + pat.rows > coor.row);
			}).ToArray();
			
			if (pats.Length > 1)
				Debug.LogError("Two share row "+coor.row+", which is current unsupported behaviour.");
			
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
				return CellType.Undefined;
			
			// Calc relative coordinates
			coor.row -= pat.origin.row;
			coor.col -= pat.origin.col;
			
			return pat.typeAtCoordinate(coor);
		}
		
		
		public int colsLeft()
		{
			if (patterns.Count == 0)
				return 0;
			
			// Find extreme left and right patterns
			int leftBound  = int.MaxValue;
			foreach (CellPattern pat in patterns) {
				
				int b = pat.origin.col - pat.colsLeft;
				if (b < leftBound)
					leftBound = b;
			}
			
			return leftBound;
		}
		
		
		public int colsRight()
		{
			if (patterns.Count == 0)
				return 0;
			
			// Find extreme left and right patterns
			int rightBound  = int.MinValue;
			foreach (CellPattern pat in patterns) {
				
				int b = pat.origin.col + pat.colsRight;
				if (b > rightBound)
					rightBound = b;
			}
			
			return rightBound;
		}
		
		
		public int numberOfColumns()
		{
			return 1 + colsRight() - colsLeft(); // Pattern has min width of 1
		}
		
		
		public int numberOfRows()
		{
			if (patterns.Count == 0)
				return 0;
			
			// Last pattern should be deepest into the level
			// Will need changing if parallel patterns are allowed
			CellPattern lastPat = patterns[patterns.Count-1];
			return lastPat.origin.row + lastPat.rows;
		}
	}
}

