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
			cv.i    =  3; cv.j    = 0;
			exp.col =  6; exp.row = 2;
			act = pc + cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(3,0) + cv(3,0) = pc(6,2)");
			Assert.AreEqual(exp.row, act.row, "row: pc(3,0) + cv(3,0) = pc(6,2)");
			
			
			// pc(5,1) + cv(2,1) = pc(7,3)
			pc.col  =  5; pc.row  = 1;
			cv.i    =  2; cv.j    = 1;
			exp.col =  7; exp.row = 3;
			act = pc + cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(5,1) + cv(2,1) = pc(7,3)");
			Assert.AreEqual(exp.row, act.row, "row: pc(5,1) + cv(2,1) = pc(7,3)");
			
			
			// pc(0,0) + cv(2,0) = pc(2,1)
			pc.col  =  0; pc.row  = 0;
			cv.i    =  2; cv.j    = 0;
			exp.col =  2; exp.row = 1;
			act = pc + cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(0,0) + cv(2,0) = pc(2,1)");
			Assert.AreEqual(exp.row, act.row, "row: pc(0,0) + cv(2,0) = pc(2,1)");
			
			
			// pc(0,0) + cv(3,0) = pc(3,1)
			pc.col  =  0; pc.row  =  0;
			cv.i    =  3; cv.j    =  0;
			exp.col =  3; exp.row =  1;
			act = pc + cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(0,0) + cv(3,0) = pc(3,1)");
			Assert.AreEqual(exp.row, act.row, "row: pc(0,0) + cv(3,0) = pc(3,1)");
			
			
			// pc(0,0) - cv(1,0) = pc(-1,-1)
			pc.col  =  0; pc.row  =  0;
			cv.i    =  1; cv.j    =  0;
			exp.col = -1; exp.row = -1;
			act = pc - cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(0,0) - cv(1,0) = pc(-1,-1)");
			Assert.AreEqual(exp.row, act.row, "row: pc(0,0) - cv(1,0) = pc(-1,-1)");
			
			
			// pc(0,0) - cv(2,0) = pc(-2,-1)
			pc.col  =  0; pc.row  =  0;
			cv.i    =  2; cv.j    =  0;
			exp.col = -2; exp.row = -1;
			act = pc - cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(0,0) - cv(2,0) = pc(-2,-1)");
			Assert.AreEqual(exp.row, act.row, "row: pc(0,0) - cv(2,0) = pc(-2,-1)");
			
			
			// pc(1,0) - cv(2,0) = pc(-1,-1)
			pc.col  =  1; pc.row  =  0;
			cv.i    =  2; cv.j    =  0;
			exp.col = -1; exp.row = -1;
			act = pc - cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(1,0) - cv(2,0) = pc(-1,-1)");
			Assert.AreEqual(exp.row, act.row, "row: pc(1,0) - cv(2,0) = pc(-1,-1)");
			
			
			// pc(1,2) - cv(5,0) = pc(-4,0)
			pc.col  =  1; pc.row  =  2;
			cv.i    =  5; cv.j    =  0;
			exp.col = -4; exp.row =  0;
			act = pc - cv;
			Assert.AreEqual(exp.col, act.col, "col: pc(1,2) - cv(5,0) = pc(-4,0)");
			Assert.AreEqual(exp.row, act.row, "row: pc(1,2) - cv(5,0) = pc(-4,0)");
			
			
			// (pc(7,10) + cv(3,12)) - cv(3,12) = pc(7,10)
			pc.col  =  7; pc.row  = 10;
			cv.i    =  3; cv.j    = 13;
			exp.col =  7; exp.row = 10;
			act = (pc + cv) - cv;
			Assert.AreEqual(exp.col, act.col, "col: (pc(7,10) + cv(3,12)) - cv(3,12) = pc(7,10)");
			Assert.AreEqual(exp.row, act.row, "row: (pc(7,10) + cv(3,12)) - cv(3,12) = pc(7,10)");
			
			
			// (pc(2,10) - cv(8,99)) + cv(8,99) = pc(2,10)
			pc.col  =  2; pc.row  = 10;
			cv.i    =  8; cv.j    = 99;
			exp.col =  2; exp.row = 10;
			act = (pc - cv) + cv;
			Assert.AreEqual(exp.col, act.col, "col: (pc(2,10) - cv(8,99)) + cv(8,99) = pc(2,10)");
			Assert.AreEqual(exp.row, act.row, "row: (pc(2,10) - cv(8,99)) + cv(8,99) = pc(2,10)");
			
			
		}
		
		
		[Test]
		public void traversal()
		{
			int[] exps = {
				5, 2,
				6, 2,
				7, 1,
				8, 1
			};
			PatternCoordinate pc = new PatternCoordinate(0, 0) + new CellVector(5, 0);
			for (int i=0; i<exps.Length/2; i+=2) {
				Assert.AreEqual(exps[i  ], pc.col, "col: i="+i);
				Assert.AreEqual(exps[i+1], pc.row, "row: i="+i);
				pc += new CellVector(1, -1);
			}
		}
		
		
		[Test]
		public void referenceFrameEquivalence()
		{
			CellVector cv1a = new CellVector(CellIndex.topMiddle, 10);
			CellVector cv1b = new CellVector(CellIndex.topLeft, 8);
			PatternCoordinate pc1 = new PatternCoordinate(0, 0) + (cv1a - cv1b);
			PatternCoordinate pc2 = (new PatternCoordinate(0, 0) + cv1a) - cv1b;
			Assert.AreEqual(pc1.col, pc2.col);
			Assert.AreEqual(pc1.row, pc2.row);
			
			
			cv1a = new CellVector(2, -1);
			cv1b = new CellVector(CellIndex.topLeft, 8);
			pc1 = new PatternCoordinate(0, 0) + (cv1a - cv1b);
			pc2 = (new PatternCoordinate(0, 0) + cv1a) - cv1b;
			Assert.AreEqual(pc1.col, pc2.col);
			Assert.AreEqual(pc1.row, pc2.row);
			
		}
	}
}