using System;
using System.Collections.Generic;

namespace Penguin
{
	[Serializable]
	public class HolesPattern : CellPattern
	{
		public int size;
		public int separation;
		
		public override Dictionary<string, object> packDict ()
		{
			Dictionary<string, object> data = base.packDict ();
			
			data["size"      ] = size;
			data["separation"] = separation;
			
			return data;
		}
		
		
		public override void unpackDict (Dictionary<string, object> data)
		{
			base.unpackDict (data);
			
			size       = Convert.ToInt32(data["size"      ]);
			separation = Convert.ToInt32(data["separation"]);
		}
		
		
		public override CellType typeAtCoordinate (PatternCoordinate coor)
		{
			// add diagonal displacement
			coor.col += coor.row / separation;
			
			int sectorSize = size + separation;
			
			// Inside column hole?
			coor.col = Math.Abs (coor.col) % sectorSize;
			if (coor.col < size) {
				
				// Inside row hole?
				coor.row = Math.Abs (coor.row) % sectorSize;
				if (coor.row < size)
					return CellType.Empty;
			}
			
			return CellType.Normal;
		}
	}
}

