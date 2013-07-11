using NUnit.Framework;
using System;
using Penguin;

namespace PenguinTest
{
	[TestFixture()]
	public class CellIndexTest
	{
		[Test()]
		public void TypeCasting ()
		{
			CellIndex c = new CellIndex(0);
			Assert.IsInstanceOfType(typeof(CellIndex), 1+c, "CellIndex + int should return a CellIndex");
		}
		
		
		[Test()]
		public void Arithmetic ()
		{
			CellIndex c1 = new CellIndex(4);
			CellIndex c2 = new CellIndex(0) - 2;
			Assert.AreEqual(c1, c2, "CellIndex(0)-2 should equal 4, but is instead "+c2);
		}
		
		
	}
}

