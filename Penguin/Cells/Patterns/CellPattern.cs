using System;

namespace Penguin
{
	public abstract class CellPattern
	{
		// This is how the CellMap will decide what to build
		// Coordinates representation:
		//   ---         ---         ---
		// /     \     /     \     /     \
		// -2, 3   ---  0, 2   ---  2, 1   
		// \     /     \     /     \     /
		//   --- -1, 2   ---  1, 1   ---
		// /     \     /     \     /     \
		// -2, 2   ---  0, 1   ---  2, 0   
		// \     /     \     /     \     /
		//   --- -1, 1   ---  1, 0   ---
		// /     \     /     \     /     \
		// -2, 1   ---  0, 0   ---  2, -1   
		// \     /     \     /     \     /
		//   --- -1, 0   ---  1, -1  ---
		// /     \     /     \     /     \
		// -2, 0   ---  0, -1  ---  2, -2  
		// \     /     \     /     \     /
		//   ---         ---         ---
		// coordinate (0, 0) is the pattern's ideal entry point
		public abstract CellType typeAtIndex(int row, int column);
		
		// Some patterns are easier to design in alternate coordinates
		// Alternate system - wavy coordinates:
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
		protected void convertToWavy(ref int row, ref int column)
		{
			// column value doesn't change, put intermediates elsewhere
			int x = column;
			
			// For odd columns subtract 1
			if (column%2 != 0) x--;
			
			row += x/2;
		}
	}
}

