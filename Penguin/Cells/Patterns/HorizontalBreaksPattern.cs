using System;

namespace Penguin
{
	public class HorizontalBreaksPattern : CellPattern
	{
		
		// Our algorithm parameters
		private int breakSize_;
		private int intervalSize_;
		
		public HorizontalBreaksPattern (int breakSize, int intervalSize)
		{
			breakSize_    = breakSize;
			intervalSize_ = intervalSize;
		}
		
		
		// The pattern forming algorithm
		public override CellType typeAtCoordinate(PatternCoordinate coor)
		{
			// Column is ignored, only interested in row
			
			// Length of repeating pattern segment
			int segmentLength = breakSize_ + intervalSize_;
			
			// Determine relative index to repeating segment
			// Can't use modulo, as index isn't guaranteed positive
			while (coor.row < 0)             coor.row += segmentLength;
			while (coor.row > segmentLength) coor.row -= segmentLength;
			
			// Choose cell type
			if (coor.row < intervalSize_)
				return CellType.Normal;
			else
				return CellType.Empty;
		}
	}
}

