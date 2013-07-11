using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
	public class CellMap
	{
		
		// Including the central cell, this hexagon shaped map's radius is
		// equal to the length of it's sides
		private int radius_;
		public int radius {get{return radius_;}}
		
		// Used to calculate cells relative positions
		private float cellSize_;
		
		// Referenced when instaniating cells
		private Dictionary<CellType, GameObject> platformDict_;
		
		
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
		
		
		// Needed to find relative position to player
		public Vector2 centre {
			get{
				if (tlCell_.platform==null)
					Debug.LogError("CellMap centre undefined before instantiation");
				Vector2 vec = new Vector2(tlCell_.platform.transform.position.x,
										  tlCell_.platform.transform.position.z);
				vec -= (radius_-1) * cellSize_ * Vector2.up;
				return vec;
			}
		}
		
		
		public CellMap (int radius,
						float cellSize,
						Dictionary<CellType, GameObject> platfromDict)
		{
			if (radius<1) Debug.LogError("CellMap cannot have radius below 1");
			radius_       = radius;
			cellSize_     = cellSize;
			platformDict_ = platfromDict;
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
			initialCellsAreBuilt_ = true;
			
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
		
		
		private void instantiateCell(Cell cell, Vector2 position)
		{
			if (cell.type == CellType.Empty) return;
			if (!platformDict_.ContainsKey(cell.type)) {
				Debug.LogError("CellMap's platformDict contains no gameObject for key: "+cell.type);
				return;
			}
			
			Vector3 pos = new Vector3(position.x, 0.0f, position.y);
			cell.platform = (GameObject)GameObject.Instantiate(platformDict_[cell.type], pos, Quaternion.identity);
		}
		
		
		private bool initialCellsAreInstantiated_;
		public void instantiateInitialCells(Vector2 centre)
		{
			if (initialCellsAreInstantiated_) Debug.LogError("Cells have been instantiated once already");
			initialCellsAreInstantiated_ = true;
			
			// Cell traversal vectors
			Vector2 rightUp = new Vector2(cellSize_*Mathf.Sin(Mathf.PI/3.0f),
												cellSize_*Mathf.Cos(Mathf.PI/3.0f));
			Vector2 down = -cellSize_ * Vector2.up;
			
			Vector2 firstPosOfRow = ((radius_-1) * (-rightUp-down)) + centre;
			Cell firstCellOfRow = tlCell_;
			while (firstCellOfRow!=null) {
				
				Cell    c = firstCellOfRow;
				Vector2 v = firstPosOfRow;
				while (c!=null) {
					instantiateCell(c, v);
					// Traverse up and right
					c = c[1];
					v += rightUp;
				}
				
				// Traverse left side then bottom-left side
				if (firstCellOfRow[3]!=null) {
					firstCellOfRow = firstCellOfRow[3];
					firstPosOfRow += down;
				} else {
					firstCellOfRow = firstCellOfRow[2];
					firstPosOfRow += down + rightUp;
				}
			}
		}
		
		
	}
	
	
}

