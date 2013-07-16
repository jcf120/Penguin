using System;

namespace Penguin
{
	public abstract class CellPattern
	{
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
		public abstract CellType typeAtIndex(int row, int column);
	}
}
