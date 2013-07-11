using NUnit.Framework;
using System;
using Penguin;

namespace PenguinTest
{
	[TestFixture()]
	public class CellMapTest
	{
		private CellMap cellMap;
		
		[SetUp()]
		public void Init()
		{
			cellMap = new CellMap(3);
		}
		
		[Test()]
		// Check perimeter points outwards to null
		// Also check entry points are bridged
		public void TestPerimeter ()
		{
			Cell c = cellMap.topMiddleCell;
			int r = cellMap.radius;
			
			// Top right side
			for (int i=0; i<r; i++) {
				Assert.IsNull(c[0], "Top right side cell "+i+" connected at index 0");
				Assert.IsNull(c[1], "Top right side cell "+i+" connected at index 1");
				if (i<r-1) c = c[2];
			}
			
			Assert.AreSame(cellMap.topRightCell, c, "Top Middle doesn't bridge to Top Right");
			
			// Right side
			for (int i=0; i<r; i++) {
				Assert.IsNull(c[1], "Right side cell "+i+" connected at index 1");
				Assert.IsNull(c[2], "Right side cell "+i+" connected at index 2");
				if (i<r-1) c = c[3];
			}
			
			Assert.AreSame(cellMap.bottomRightCell, c, "Top Right doesn't bridge to Bottom Right");
			
			// Bottom right side
			for (int i=0; i<r; i++) {
				Assert.IsNull(c[2], "Bottom right side cell "+i+" connected at index 2");
				Assert.IsNull(c[3], "Bottom right side cell "+i+" connected at index 3");
				if (i<r-1) c = c[4];
			}
			
			Assert.AreSame(cellMap.bottomMiddleCell, c, "Bottom Right doesn't bridge to Bottom Middle");
			
			// Bottom left side
			for (int i=0; i<r; i++) {
				Assert.IsNull(c[3], "Bottom left side cell "+i+" connected at index 3");
				Assert.IsNull(c[4], "Bottom left side cell "+i+" connected at index 4");
				if (i<r-1) c = c[5];
			}
			
			Assert.AreSame(cellMap.bottomLeftCell, c, "Bottom Middle doesn't bridge to Bottom Left");
			
			// Left side
			for (int i=0; i<r; i++) {
				Assert.IsNull(c[4], "Left side cell "+i+" connected at index 4");
				Assert.IsNull(c[5], "Left side cell "+i+" connected at index 5");
				if (i<r-1) c = c[0];
			}
			
			Assert.AreSame(cellMap.topLeftCell, c, "Bottom Left doesn't bridge to Top Right");
			
			// Top left side
			for (int i=0; i<r; i++) {
				Assert.IsNull(c[5], "Top right side cell "+i+" connected at index 5");
				Assert.IsNull(c[0], "Top right side cell "+i+" connected at index 0");
				if (i<r-1) c = c[1];
			}
			
			Assert.AreSame(cellMap.topMiddleCell, c, "Top Left doesn't bridge to Top Middle");
		}
	}
}

