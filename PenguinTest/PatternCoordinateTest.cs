using NUnit.Framework;
using System;
using Penguin;

namespace PenguinTest
{
	[TestFixture()]
	public class PatternCoordinateTest
	{
		[Test()]
		public void TestArithmetic ()
		{
			PatternCoordinate  pc = new PatternCoordinate(0, 0);
			CellVector         cv = new CellVector(0, 0);
			PatternCoordinate exp = new PatternCoordinate(0, 0);
			PatternCoordinate act = new PatternCoordinate(0, 0);
			
			
			// pc(3,0) + cv(3,0) = pc(6,2)
			pc.col  =  3; pc.row  = 0;
			cv.x    =  3; cv.y    = 0;
			exp.col =  6; exp.row = 2;
			act = pc + cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(3,0) + cv(3,0) = pc(6,2)");
			Assert.AreEqual(exp.row, act.row, "row: pc(3,0) + cv(3,0) = pc(6,2)");
			
			
			// pc(5,1) + cv(2,1) = pc(7,3)
			pc.col  =  5; pc.row  = 1;
			cv.x    =  2; cv.y    = 1;
			exp.col =  7; exp.row = 3;
			act = pc + cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(5,1) + cv(2,1) = pc(7,3)");
			Assert.AreEqual(exp.row, act.row, "row: pc(5,1) + cv(2,1) = pc(7,3)");
			
			
			// pc(0,0) + cv(2,0) = pc(2,1)
			pc.col  =  0; pc.row  = 0;
			cv.x    =  2; cv.y    = 0;
			exp.col =  2; exp.row = 1;
			act = pc + cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(0,0) + cv(2,0) = pc(2,1)");
			Assert.AreEqual(exp.row, act.row, "row: pc(0,0) + cv(2,0) = pc(2,1)");
			
			
			// pc(0,0) - cv(1,0) = pc(-1,-1)
			pc.col  =  0; pc.row  =  0;
			cv.x    =  1; cv.y    =  0;
			exp.col = -1; exp.row = -1;
			act = pc - cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(0,0) - cv(1,0) = pc(-1,-1)");
			Assert.AreEqual(exp.row, act.row, "row: pc(0,0) - cv(1,0) = pc(-1,-1)");
			
			
			// pc(0,0) - cv(2,0) = pc(-2,-1)
			pc.col  =  0; pc.row  =  0;
			cv.x    =  2; cv.y    =  0;
			exp.col = -2; exp.row = -1;
			act = pc - cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(0,0) - cv(2,0) = pc(-2,-1)");
			Assert.AreEqual(exp.row, act.row, "row: pc(0,0) - cv(2,0) = pc(-2,-1)");
			
			
			// (pc(7,10) + cv(3,12)) - cv(3,12) = pc(7,10)
			pc.col  =  7; pc.row  = 10;
			cv.x    =  3; cv.y    = 13;
			exp.col =  7; exp.row = 10;
			act = (pc + cv) - cv;
			Assert.AreEqual(exp.col, act.col, "col: (pc(7,10) + cv(3,12)) - cv(3,12) = pc(7,10)");
			Assert.AreEqual(exp.row, act.row, "row: (pc(7,10) + cv(3,12)) - cv(3,12) = pc(7,10)");
		}
	}
}