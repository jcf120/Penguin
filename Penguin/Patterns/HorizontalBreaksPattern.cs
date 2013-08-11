using System;
using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
	[Serializable]
	public class HorizontalBreaksPattern : CellPattern
	{
		
		// Our algorithm parameters
		public int breakSize;
		public int intervalSize;
		
		public override Dictionary<string, object> packDict ()
		{
			Dictionary<string, object> data = base.packDict ();
			
			data["breakSize"   ] = breakSize;
			data["intervalSize"] = intervalSize;
			
			return data;
		}
		
		
		public override void unpackDict (Dictionary<string, object> data)
		{
			base.unpackDict (data);
			
			breakSize    = Convert.ToInt32(data["breakSize"   ]);
			intervalSize = Convert.ToInt32(data["intervalSize"]);
		}
		
		
		// The pattern forming algorithm
		public override CellType typeAtCoordinate(PatternCoordinate coor)
		{
			// Column is ignored, only interested in row
			
			// Length of repeating pattern segment
			int segmentLength = breakSize + intervalSize;
			
			// Determine relative index to repeating segment
			// Can't use modulo, as index isn't guaranteed positive
			while (coor.row < 0)             coor.row += segmentLength;
			while (coor.row > segmentLength) coor.row -= segmentLength;
			
			// Choose cell type
			if (coor.row < intervalSize)
				return CellType.Normal;
			else
				return CellType.Empty;
		}
	}
}

