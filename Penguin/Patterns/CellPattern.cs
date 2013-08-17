using System;
using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
	[Serializable]
	public abstract class CellPattern : ScriptableObject
	{
		public int originCol = 0;
		public int originRow = 0;
		public PatternCoordinate origin
		{
			get { return new PatternCoordinate(originCol,originRow); }
			set { originCol = value.col; originRow = value.row; }
		}
		public int rows      = 5;
		public int colsLeft  = 5;
		public int colsRight = 5;
		// This is how the CellMap will decide what to build
		// Coordinates representation - wavy:
		//   ---         ---         ---
		// /     \     /     \     /     \
		// -2, 2   ---  0, 2   ---  2, 2   
		// \     /     \     /     \     /
		//   --- -1, 1   ---  1, 1   ---
		// /     \     /     \     /     \
		// -2, 1   ---  0, 1   ---  2, 1   
		// \     /     \     /     \     /
		//   --- -1, 0   ---  1, 0   ---
		// /     \     /     \     /     \
		// -2, 0   ---  0, 0   ---  2, 0   
		// \     /     \     /     \     /
		//   --- -1, -1  ---  1, -1  ---
		// /     \     /     \     /     \
		// -2, -1  ---  0, -1  ---  2, -1  
		// \     /     \     /     \     /
		//   ---         ---         ---
		// coordinate (0, 0) is the pattern's ideal entry point
		public abstract CellType typeAtCoordinate(PatternCoordinate coor);
		
		// json serialisation bridge methods
		
		virtual public Dictionary<string,object> packDict()
		{
			Dictionary<string,object> data = new Dictionary<string, object>();
			
			data["class"    ] = GetType().ToString();
			data["originCol"] = origin.col;
			data["originRow"] = origin.row;
			data["colsLeft" ] = colsLeft;
			data["colsRight"] = colsRight;
			data["rows"     ] = rows;
			
			return data;
		}
		
		
		virtual public void unpackDict(Dictionary<string,object> data)
		{
			if (data["class"].ToString() != GetType().ToString()) {
				Debug.LogError("Data used to unpack "+GetType().ToString()+" doesn't represent class. Unpacking aborted");
				return;
			}
			
			originCol = Convert.ToInt32(data["originCol"]);
			originRow = Convert.ToInt32(data["originRow"]);
			colsLeft  = Convert.ToInt32(data["colsLeft" ]);
			colsRight = Convert.ToInt32(data["colsRight"]);
			rows      = Convert.ToInt32(data["rows"     ]);
		}
	}
}

