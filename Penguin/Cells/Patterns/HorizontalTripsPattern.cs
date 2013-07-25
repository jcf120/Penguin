using System;

namespace Penguin
{
	
	public class HorizontalTripsPattern: CellPattern
	{
		private int separation_;
		
		public HorizontalTripsPattern (int separation)
		{
			separation_ = separation;
		}
		
		
		public override CellType typeAtCoordinate (PatternCoordinate coor)
		{
			coor.row %= separation_;
			if (coor.row == 0)
				return CellType.TripHazard;
			else
				return CellType.Normal;
		}
	}
}

