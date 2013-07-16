using System;

namespace Penguin
{
	public struct PatternCoordinate
	{
		public int col;
		public int row;
		
		
		public PatternCoordinate(int c, int r)
		{
			col = c;
			row = r;
		}
		
		
		public static PatternCoordinate operator+ (PatternCoordinate pc, CellVector cv)
		{
			int i = cv.i;
			if (i%2 != 0) i+=i%2;
			pc.row += i/2 + cv.j;
			pc.col += cv.i;
			return pc;
		}
		
		
		public static PatternCoordinate operator- (PatternCoordinate pc, CellVector cv)
		{
			cv.i = -cv.i;
			cv.j = -cv.j;
			return pc + cv;
		}
	}
}

