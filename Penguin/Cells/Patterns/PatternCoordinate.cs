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
			int i = cv.x;
			if (i%2 != 0) i+=i%2;
			pc.row += i/2 + cv.y;
			pc.col += cv.x;
			return pc;
		}
		
		
		public static PatternCoordinate operator- (PatternCoordinate pc, CellVector cv)
		{
			cv.x = -cv.x;
			cv.y = -cv.y;
			return pc + cv;
		}
	}
}

