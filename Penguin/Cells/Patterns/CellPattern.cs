using System;

namespace Penguin
{
	public abstract class CellPattern
	{
		// This is how the CellMap will decide what to build
		public abstract CellType typeAtIndex(int row, int column);
		
		
	}
}

