using System;

namespace Penguin
{
	public class HolesPattern : CellPattern
	{
		private int size_;
		private int separation_;
		
		public HolesPattern (int size, int separation)
		{
			size_     = size;
			separation_ = separation;
		}
		
		
		public override CellType typeAtCoordinate (PatternCoordinate coor)
		{
			// add diagonal displacement
			coor.col += coor.row / separation_;
			
			int sectorSize = size_ + separation_;
			
			// Inside column hole?
			coor.col = Math.Abs (coor.col) % sectorSize;
			if (coor.col < size_) {
				
				// Inside row hole?
				coor.row = Math.Abs (coor.row) % sectorSize;
				if (coor.row < size_)
					return CellType.Empty;
			}
			
			return CellType.Normal;
		}
	}
}

