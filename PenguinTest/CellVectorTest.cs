using NUnit.Framework;
using System;
using Penguin;

namespace PenguinTest
{
	[TestFixture()]
	public class CellVectorTest
	{
		[Test()]
		public void rotation ()
		{
			CellVector cv  = new CellVector(0, 0);
			CellVector exp = new CellVector(0, 0);
			CellVector act = new CellVector(0, 0);
			
			
			// cv(0,10).rot(1) = cv(10,0)
			cv.i  = 0;  cv.j  = 10;
			exp.i = 10; exp.j = 0;
			act = cv.rotated(CellIndex.topRight);
			Assert.AreEqual(exp.i, act.i, "x: cv(0,10).rot(1) = cv(10,0)");
			Assert.AreEqual(exp.j, act.j, "y: cv(0,10).rot(1) = cv(10,0)");
			
			
			// cv(1,1).rot(2) = cv(2,-1)
			cv.i  = 1;  cv.j  = 1;
			exp.i = 1;  exp.j = -2;
			act = cv.rotated(CellIndex.bottomRight);
			Assert.AreEqual(exp.i, act.i, "x: cv(1,1).rot(2) = cv(2,-1)");
			Assert.AreEqual(exp.j, act.j, "y: cv(1,1).rot(2) = cv(2,-1)");
			
			
			// cv(1,1).rot(3) = cv(-1,-1)
			cv.i  = 1;  cv.j  = 1;
			exp.i = -1; exp.j = -1;
			act = cv.rotated(CellIndex.bottomMiddle);
			Assert.AreEqual(exp.i, act.i, "x: cv(1,1).rot(3) = cv(-1,-1)");
			Assert.AreEqual(exp.j, act.j, "y: cv(1,1).rot(3) = cv(-1,-1)");
			
			
			// cv(1,1).rot(4) = cv(-2,1)
			cv.i  = 1;  cv.j  = 1;
			exp.i = -2; exp.j = 1;
			act = cv.rotated(CellIndex.bottomLeft);
			Assert.AreEqual(exp.i, act.i, "x: cv(1,1).rot(4) = cv(-2,1)");
			Assert.AreEqual(exp.j, act.j, "y: cv(1,1).rot(4) = cv(-2,1)");
			
			
			// cv(1,1).rot(5) = cv(2,-1)
			cv.i  = 1;  cv.j  = 1;
			exp.i = -1; exp.j = 2;
			act = cv.rotated(CellIndex.topLeft);
			Assert.AreEqual(exp.i, act.i, "x: cv(1,1).rot(5) = cv(2,-1)");
			Assert.AreEqual(exp.j, act.j, "y: cv(1,1).rot(5) = cv(2,-1)");
			
			
			// cv(11,5).rot(1).rot(3).rot(2) = cv(11,5)
			cv.i  = 11;  cv.j = 5;
			exp.i = 11; exp.j = 5;
			act = cv.rotated(CellIndex.topRight).rotated(CellIndex.bottomMiddle).rotated(CellIndex.bottomRight);
			Assert.AreEqual(exp.i, act.i, "x: cv(11,5).rot(1).rot(3).rot(2) = cv(11,5)");
			Assert.AreEqual(exp.j, act.j, "y: cv(11,5).rot(1).rot(3).rot(2) = cv(11,5)");
		}
	}
}

