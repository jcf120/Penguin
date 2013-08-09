using System;

namespace Penguin
{
	[Serializable]
	public class HolesPattern : CellPattern
	{
		public int size;
		public int separation;
		
		public HolesPattern ()
		{
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

