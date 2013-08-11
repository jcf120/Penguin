using System;
using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
	[Serializable]
	public abstract class CellPattern : ScriptableObject
	{
		public PatternCoordinate origin;
		public int rows;
		public int colsLeft;
		public int colsRight;
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
			origin.col = (int)data["originCol"];
			origin.row = (int)data["originRow"];
			colsLeft   = (int)data["colsLeft" ];
			colsRight  = (int)data["colsRight"];
			rows       = (int)data["rows"     ];
		}
	}
}

