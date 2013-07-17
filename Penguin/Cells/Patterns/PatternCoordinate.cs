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
			int i = Math.Abs(cv.i%2);
			pc.row += cv.j + ((cv.i+((2*(pc.col%2)-1)*i))/2);
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

