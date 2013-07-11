using System;
using UnityEngine;

namespace Penguin
{
	public class CellMap
	{
		
		// Including the central cell, this hexagon shaped map's radius is
		// equal to the length of it's sides
		private int radius_;
		public int radius {get{return radius_;}}
		
		
		// Entry points into map
		private Cell tlCell_ = null; // top left corner
		private Cell tmCell_ = null; // top middle corner
		private Cell trCell_ = null; // top right corner
		private Cell blCell_ = null; // bottom left corner
		private Cell bmCell_ = null; // bottom middle corner
		private Cell brCell_ = null; // bottom right corner
		// Accessors
		public Cell topLeftCell      {get{return tlCell_;}}
		public Cell topMiddleCell    {get{return tmCell_;}}
		public Cell topRightCell     {get{return trCell_;}}
		public Cell bottomLeftCell   {get{return blCell_;}}
		public Cell bottomMiddleCell {get{return bmCell_;}}
		public Cell bottomRightCell  {get{return brCell_;}}
		
		
		public CellMap (int radius)
		{
			if (radius<1) Debug.LogError("CellMap cannot have radius below 1");
			radius_ = radius;
			buildInitialCells(CellType.Normal);
		}
		
		
		// Mutually joins c2 at specified index of c1
		private void linkCells(Cell c1, Cell c2, int c1index)
		{
			// Calculate corresponding side of c2
			int c2index = c1index - 3;
			if (c2index<0) c2index += 6;
			
			// Check links aren't already occupied
			if (c1[c1index]!=null) Debug.LogError("Can't assign to already occupied link " + c1index + " in c1");
			if (c2[c2index]!=null) Debug.LogError("Can't assign to already occupied link " + c1index + " in c2");
			c1[c1index] = c2;
			c2[c2index] = c1;
		}
		
		
		private bool initialCellsAreBuilt_ = false;
		private void buildInitialCells(CellType type)
		{
			if (initialCellsAreBuilt_) Debug.LogError("Cells have been built once already");
			
			// Start building from top-left corner
			tlCell_ = new Cell(type);
			
			// Build row diagonally upwards, linking backwards
			Cell prev = tlCell_;
			for (int i=1; i<radius_; i++) {
				Cell c = new Cell(type);
				// Link bottom left corner
				linkCells(c, prev, 4);
				prev = c;
			}
			
			// Reference top middle entry point
			tmCell_ = prev;
			
			// Build rows underneath until we reach the top-right corner
			Cell firstOfPreviousRow = tlCell_;
			for (int i=1; i<radius_; i++) {
				// First cell of row
				prev = new Cell(type);
				// Link to cell above
				Cell abovePrev = firstOfPreviousRow;
				linkCells(prev, abovePrev, 0);
				
				// Remaining cells in row
				for (int j=1; j<radius_+i; j++) {
					Cell c = new Cell(type);
					// Link bottom left
					linkCells(c, prev, 4);
					// Link top left
					linkCells(c, abovePrev, 5);
					// Link above, unless last cell in row
					if (j<radius_+i-1) {
						abovePrev = abovePrev[1];
						linkCells(c, abovePrev, 0);
					}
					prev = c;
				}
			
				// Reference for next row iteration
				firstOfPreviousRow = firstOfPreviousRow[3];
			}
		
			// Reference entry points from previous row
			blCell_ = firstOfPreviousRow;
			trCell_ = prev;
			
			// Build rows underneath until we reach the bottom-right corner
			for (int i=radius_-2; i>=0; i--) {
				
				// First cell of row
				prev = new Cell(type);
				// Link top left corner
				linkCells(prev, firstOfPreviousRow, 5);
				// Link above
				Cell abovePrev = firstOfPreviousRow[1];
				linkCells(prev, abovePrev, 0);
				
				// Build remaining cells in row
				for (int j=1; j<radius_+i; j++) {
					Cell c = new Cell(type);
					// Link bottom left
					linkCells(c, prev, 4);
					// Link top left
					linkCells(c, abovePrev, 5);
					// Link above
					abovePrev = abovePrev[1];
					linkCells(c, abovePrev, 0);
					prev = c;
				}
				
				// Reference for next row iteration
				firstOfPreviousRow = firstOfPreviousRow[2];
			}
			
			// Reference entry points from final row
			bmCell_ = firstOfPreviousRow;
			brCell_ = prev;
		}
		
		
		private void instantiateInitialCells()
		{
			
		}
	}
	
	
}

