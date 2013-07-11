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
	}
}

