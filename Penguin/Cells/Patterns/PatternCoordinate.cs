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
			/*int i = cv.i;
			if ((i+pc.col)%2 != 0) i+=(i+pc.col)%2;
			pc.row += i/2 + cv.j;
			pc.col += cv.i;*/
			/*if (pc.col%2==0) { // even column
				if (cv.i<0) { // negative i
					int i = cv.i%2;
					pc.row += ((cv.i+i)/2) + cv.j;
				} else { // positive i
					int i = cv.i%2;
					pc.row += ((cv.i-i)/2) + cv.j;
				}
			} else { // odd column
				if (cv.i<0) { // negative i
					int i = cv.i%2;
					pc.row += ((cv.i-i)/2) + cv.j;
				} else { // positive i
					int i = cv.i%2;
					pc.row += ((cv.i+i)/2) + cv.j;
				}
			}*/
			int i = Math.Abs(cv.i%2);
			if (pc.col%2==0) { // even column
				pc.row += cv.j + ((cv.i-i)/2);
			} else {           // odd column
				pc.row += cv.j + ((cv.i+i)/2);
			}
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

