using System;

namespace Penguin
{
	public class SmallHolesPattern : CellPattern
	{
		private float prob_;
		
		public SmallHolesPattern (float probablity)
		{
			prob_ = probablity;
		}
		
		
		public override CellType typeAtCoordinate (PatternCoordinate coor)
		{
			// Not truely random so that referncing same coordinate
			// gives same result, like a hashcode
			// Could provide a seed later
			Random rand = new Random(coor.col * coor.row);
			if (rand.NextDouble() + prob_ > 1.0f)
				return CellType.Empty;
			else
				return CellType.Normal;
		}
	}
}

