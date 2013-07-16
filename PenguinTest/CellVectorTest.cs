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
			cv.x  = 0;  cv.y  = 10;
			exp.x = 10; exp.y = 0;
			act = cv.rotated(CellIndex.topRight);
			Assert.AreEqual(exp.x, act.x, "x: cv(0,10).rot(1) = cv(10,0)");
			Assert.AreEqual(exp.y, act.y, "y: cv(0,10).rot(1) = cv(10,0)");
			
			
			// cv(1,1).rot(2) = cv(2,-1)
			cv.x  = 1;  cv.y  = 1;
			exp.x = 1;  exp.y = -2;
			act = cv.rotated(CellIndex.bottomRight);
			Assert.AreEqual(exp.x, act.x, "x: cv(1,1).rot(2) = cv(2,-1)");
			Assert.AreEqual(exp.y, act.y, "y: cv(1,1).rot(2) = cv(2,-1)");
			
			
			// cv(1,1).rot(3) = cv(-1,-1)
			cv.x  = 1;  cv.y  = 1;
			exp.x = -1; exp.y = -1;
			act = cv.rotated(CellIndex.bottomMiddle);
			Assert.AreEqual(exp.x, act.x, "x: cv(1,1).rot(3) = cv(-1,-1)");
			Assert.AreEqual(exp.y, act.y, "y: cv(1,1).rot(3) = cv(-1,-1)");
			
			
			// cv(1,1).rot(4) = cv(-2,1)
			cv.x  = 1;  cv.y  = 1;
			exp.x = -2; exp.y = 1;
			act = cv.rotated(CellIndex.bottomLeft);
			Assert.AreEqual(exp.x, act.x, "x: cv(1,1).rot(4) = cv(-2,1)");
			Assert.AreEqual(exp.y, act.y, "y: cv(1,1).rot(4) = cv(-2,1)");
			
			
			// cv(1,1).rot(5) = cv(2,-1)
			cv.x  = 1;  cv.y  = 1;
			exp.x = -1; exp.y = 2;
			act = cv.rotated(CellIndex.topLeft);
			Assert.AreEqual(exp.x, act.x, "x: cv(1,1).rot(5) = cv(2,-1)");
			Assert.AreEqual(exp.y, act.y, "y: cv(1,1).rot(5) = cv(2,-1)");
		}
	}
}

