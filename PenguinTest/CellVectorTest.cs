using NUnit.Framework;
using System;
using Penguin;
using UnityEngine;

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
			Assert.AreEqual(exp.i, act.i, "i: cv(0,10).rot(1) = cv(10,0)");
			Assert.AreEqual(exp.j, act.j, "j: cv(0,10).rot(1) = cv(10,0)");
			
			
			// cv(1,1).rot(2) = cv(2,-1)
			cv.i  = 1;  cv.j  = 1;
			exp.i = 1;  exp.j = -2;
			act = cv.rotated(CellIndex.bottomRight);
			Assert.AreEqual(exp.i, act.i, "i: cv(1,1).rot(2) = cv(2,-1)");
			Assert.AreEqual(exp.j, act.j, "j: cv(1,1).rot(2) = cv(2,-1)");
			
			
			// cv(1,1).rot(3) = cv(-1,-1)
			cv.i  = 1;  cv.j  = 1;
			exp.i = -1; exp.j = -1;
			act = cv.rotated(CellIndex.bottomMiddle);
			Assert.AreEqual(exp.i, act.i, "i: cv(1,1).rot(3) = cv(-1,-1)");
			Assert.AreEqual(exp.j, act.j, "j: cv(1,1).rot(3) = cv(-1,-1)");
			
			
			// cv(1,1).rot(4) = cv(-2,1)
			cv.i  = 1;  cv.j  = 1;
			exp.i = -2; exp.j = 1;
			act = cv.rotated(CellIndex.bottomLeft);
			Assert.AreEqual(exp.i, act.i, "i: cv(1,1).rot(4) = cv(-2,1)");
			Assert.AreEqual(exp.j, act.j, "j: cv(1,1).rot(4) = cv(-2,1)");
			
			
			// cv(1,1).rot(5) = cv(2,-1)
			cv.i  = 1;  cv.j  = 1;
			exp.i = -1; exp.j = 2;
			act = cv.rotated(CellIndex.topLeft);
			Assert.AreEqual(exp.i, act.i, "i: cv(1,1).rot(5) = cv(2,-1)");
			Assert.AreEqual(exp.j, act.j, "j: cv(1,1).rot(5) = cv(2,-1)");
			
			
			// cv(11,5).rot(1).rot(3).rot(2) = cv(11,5)
			cv.i  = 11;  cv.j = 5;
			exp.i = 11; exp.j = 5;
			act = cv.rotated(CellIndex.topRight).rotated(CellIndex.bottomMiddle).rotated(CellIndex.bottomRight);
			Assert.AreEqual(exp.i, act.i, "i: cv(11,5).rot(1).rot(3).rot(2) = cv(11,5)");
			Assert.AreEqual(exp.j, act.j, "j: cv(11,5).rot(1).rot(3).rot(2) = cv(11,5)");
			
			
			// cv(55,3).rot(5).rot(1).rot(-3).rot(-2).rot(-4).rot(3) = cv(5,3)
			cv.i  = 55;  cv.j = 3;
			exp.i = 55; exp.j = 3;
			act = cv.rotated(new CellIndex( 5)).rotated(new CellIndex( 1)).rotated(new CellIndex(-3))
					.rotated(new CellIndex(-2)).rotated(new CellIndex(-4)).rotated(new CellIndex( 3));
			Assert.AreEqual(exp.i, act.i, "i: cv(55,3).rot(5).rot(1).rot(-3).rot(-2).rot(-4).rot(3) = cv(5,3)");
			Assert.AreEqual(exp.j, act.j, "j: cv(55,3).rot(5).rot(1).rot(-3).rot(-2).rot(-4).rot(3) = cv(5,3)");
		}
		
		
		[Test()]
		public void vec2Conversion()
		{
			Vector2    vec2 = Vector2.zero;
			CellVector exp  = new CellVector(0, 0);
			CellVector act  = new CellVector(0, 0);
			
			
			// vec2(0,-0.51) -> cv(0,-1)
			vec2.x = 0.0f; vec2.y = -0.51f;
			exp.i  = 0;    exp.j  = -1;
			act = CellVector.fromVector2(vec2);
			Assert.AreEqual(exp.i, act.i, "i: vec2(0,-0.51) -> cv(0,-1)");
			Assert.AreEqual(exp.j, act.j, "j: vec2(0,-0.51) -> cv(0,-1)");
			
			
			// vec2(1.16,0) -> cv(2,-1)
			vec2.x = 1.16f; vec2.y = 0.0f;
			exp.i  = 2;     exp.j  = -1;
			act = CellVector.fromVector2(vec2);
			Assert.AreEqual(exp.i, act.i, "i: vec2(1.16,0) -> cv(2,-1)");
			Assert.AreEqual(exp.j, act.j, "j: vec2(1.16,0) -> cv(2,-1)");
		}
	}
}

