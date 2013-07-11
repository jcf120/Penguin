using System;

namespace Penguin
{
	public struct CellIndex
	{
		// Possible values
		public static readonly CellIndex topMiddle    = new CellIndex(0);
		public static readonly CellIndex topRight     = new CellIndex(1);
		public static readonly CellIndex bottomRight  = new CellIndex(2);
		public static readonly CellIndex bottomMiddle = new CellIndex(3);
		public static readonly CellIndex bottomLeft   = new CellIndex(4);
		public static readonly CellIndex topLeft      = new CellIndex(5);
		
		int index;
		
		
		public CellIndex(int i)
		{
			index = i;
		}
		
		
		public static CellIndex operator+ (CellIndex lhs, CellIndex rhs)
		{
			CellIndex result = new CellIndex(0);
			result.index = lhs.index + rhs.index;
			while (result.index > 5) result.index -= 6;
			return result;
		}
		
		
		public static CellIndex operator+ (CellIndex lhs, int rhs)
		{
			CellIndex result = new CellIndex(0);
			result.index = lhs.index + rhs;
			while (result.index > 5) result.index -= 6;
			return result;
		}
		
		
		public static CellIndex operator+ (int lhs, CellIndex rhs)
		{
			CellIndex result = new CellIndex(0);
			result.index = lhs + rhs.index;
			while (result.index > 5) result.index -= 6;
			return result;
		}
		
		
		public static implicit operator int(CellIndex c)
		{
			return c.index;
		}
		
		
	}
}

