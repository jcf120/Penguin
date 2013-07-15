using System;
using UnityEngine;

namespace Penguin
{
	public struct CellIndex
	{
		// Convenience values for readability
		public static readonly CellIndex topMiddle    = new CellIndex(0);
		public static readonly CellIndex topRight     = new CellIndex(1);
		public static readonly CellIndex bottomRight  = new CellIndex(2);
		public static readonly CellIndex bottomMiddle = new CellIndex(3);
		public static readonly CellIndex bottomLeft   = new CellIndex(4);
		public static readonly CellIndex topLeft      = new CellIndex(5);
		
		int index;
		
		private void rangeCheck()
		{
			while (index > 5) index -= 6;
			while (index < 0) index += 6;
		}
		
		
		public CellIndex(int i)
		{
			index = i;
			rangeCheck();
		}
		
		
		public static CellIndex fromAngle(float degrees)
		{
			CellIndex result = new CellIndex(0);
			// Allow values near 360 to round down to 0
			degrees += 30.0f;
			// Scale and quantise to index
			result.index = Mathf.FloorToInt(degrees / 60.0f);
			if (result.index > 5) result.index -= 6;
			
			result.rangeCheck();
			return result;
		}
		
		
		public static CellIndex operator+ (CellIndex lhs, CellIndex rhs)
		{
			CellIndex result = new CellIndex(0);
			result.index = lhs.index + rhs.index;
			result.rangeCheck();
			return result;
		}
		
		
		public static CellIndex operator+ (CellIndex lhs, int rhs)
		{
			CellIndex result = new CellIndex(0);
			result.index = lhs.index + rhs;
			result.rangeCheck();
			return result;
		}
		
		
		public static CellIndex operator+ (int lhs, CellIndex rhs)
		{
			CellIndex result = new CellIndex(0);
			result.index = lhs + rhs.index;
			result.rangeCheck();
			return result;
		}
		
		
		public static CellIndex operator- (CellIndex lhs, CellIndex rhs)
		{
			CellIndex result = new CellIndex(0);
			result.index = lhs.index - rhs.index;
			result.rangeCheck();
			return result;
		}
		
		
		public static CellIndex operator- (CellIndex lhs, int rhs)
		{
			CellIndex result = new CellIndex(0);
			result.index = lhs.index - rhs;
			result.rangeCheck();
			return result;
		}
		
		
		public static CellIndex operator- (int lhs, CellIndex rhs)
		{
			CellIndex result = new CellIndex(0);
			result.index = lhs - rhs.index;
			result.rangeCheck();
			return result;
		}
		
		
		public static implicit operator int(CellIndex c)
		{
			return c.index;
		}
		
		
	}
}

