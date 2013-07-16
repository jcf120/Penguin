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
		public override CellType typeAtIndex(int row, int column)
		{
			// After converting coordinates column is ignored, only interested in row
			convertToWavy(ref row, ref column);
			
			// Length of repeating pattern segment
			int segmentLength = breakSize_ + intervalSize_;
			
			// Determine relative index to repeating segment
			// Can't use modulo, as index isn't guaranteed positive
			while (row < 0)             row += segmentLength;
			while (row > segmentLength) row -= segmentLength;
			
			// Choose cell type
			if (row < intervalSize_)
				return CellType.Normal;
			else
				return CellType.Empty;
		}
	}
}

