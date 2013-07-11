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
		//    0
		//  /   \
		// 5     1
		// |     |
		// 4     2
		//  \   /
		//    3
		private Cell[] corners_ = {null, null, null, null, null, null};
		// Accessors
		public Cell topMiddleCell    {get{return corners_[0];}}
		public Cell topRightCell     {get{return corners_[1];}}
		public Cell bottomRightCell  {get{return corners_[2];}}
		public Cell bottomMiddleCell {get{return corners_[3];}}
		public Cell bottomLeftCell   {get{return corners_[4];}}
		public Cell topLeftCell      {get{return corners_[5];}}
		
		
		// Needed to find relative position to player
		public Vector2 centre {
			get{
				if (corners_[0].platform==null)
					Debug.LogError("CellMap centre undefined before instantiation");
				Vector2 vec = new Vector2(corners_[CellIndex.topMiddle].platform.transform.position.x,
										  corners_[CellIndex.topMiddle].platform.transform.position.z);
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
		private void linkCells(Cell c1, Cell c2, CellIndex c1index)
		{
			// Calculate corresponding side of c2
			CellIndex c2index = c1index - 3;
			
			// Check links aren't already occupied
			if (c1[c1index]!=null) Debug.LogError("Can't assign to already occupied link " + c1index + " in c1");
			if (c2[c2index]!=null) Debug.LogError("Can't assign to already occupied link " + c1index + " in c2");
			c1[c1index] = c2;
			c2[c2index] = c1;
		}
		
		
		// Disconnect two cells that are referencing each other
		private void linkCells(Cell c1, Cell c2, CellIndex c1index)
		{
			// Calculate corresponding side of c2
			CellIndex c2index = c1index - 3;
			
			// Check cells aren't already unlinked
			if (c1[c1index]!=c2) Debug.LogError("Cells aren't connected at specified index so can't be unlinked");
			if (c2[c2index]!=c1) Debug.LogError("Cells aren't connected at specified index so can't be unlinked");
			c1[c1index] = null;
			c2[c2index] = null;
		}
		
		
		
		private bool initialCellsAreBuilt_ = false;
		private void buildInitialCells(CellType type)
		{
			if (initialCellsAreBuilt_) Debug.LogError("Cells have been built once already");
			initialCellsAreBuilt_ = true;
			
			// Start building from top-left corner
			corners_[CellIndex.topLeft] = new Cell(type);
			
			// Build row diagonally upwards, linking backwards
			Cell prev = corners_[CellIndex.topLeft];
			for (int i=1; i<radius_; i++) {
				Cell c = new Cell(type);
				// Link bottom left corner
				linkCells(c, prev, CellIndex.bottomLeft);
				prev = c;
			}
			
			// Reference top middle entry point
			corners_[CellIndex.topMiddle] = prev;
			
			// Build rows underneath until we reach the top-right corner
			Cell firstOfPreviousRow = corners_[CellIndex.topLeft];
			for (int i=1; i<radius_; i++) {
				// First cell of row
				prev = new Cell(type);
				// Link to cell above
				Cell abovePrev = firstOfPreviousRow;
				linkCells(prev, abovePrev, CellIndex.topMiddle);
				
				// Remaining cells in row
				for (int j=1; j<radius_+i; j++) {
					Cell c = new Cell(type);
					// Link bottom left
					linkCells(c, prev, CellIndex.bottomLeft);
					// Link top left
					linkCells(c, abovePrev, CellIndex.topLeft);
					// Link above, unless last cell in row
					if (j<radius_+i-1) {
						abovePrev = abovePrev[1];
						linkCells(c, abovePrev, CellIndex.topMiddle);
					}
					prev = c;
				}
			
				// Reference for next row iteration
				firstOfPreviousRow = firstOfPreviousRow[3];
			}
		
			// Reference entry points from previous row
			corners_[CellIndex.bottomLeft] = firstOfPreviousRow;
			corners_[CellIndex.topRight] = prev;
			
			// Build rows underneath until we reach the bottom-right corner
			for (int i=radius_-2; i>=0; i--) {
				
				// First cell of row
				prev = new Cell(type);
				// Link top left corner
				linkCells(prev, firstOfPreviousRow, CellIndex.topLeft);
				// Link above
				Cell abovePrev = firstOfPreviousRow[1];
				linkCells(prev, abovePrev, CellIndex.topMiddle);
				
				// Build remaining cells in row
				for (int j=1; j<radius_+i; j++) {
					Cell c = new Cell(type);
					// Link bottom left
					linkCells(c, prev, CellIndex.bottomLeft);
					// Link top left
					linkCells(c, abovePrev, CellIndex.topLeft);
					// Link above
					abovePrev = abovePrev[1];
					linkCells(c, abovePrev, CellIndex.topMiddle);
					prev = c;
				}
				
				// Reference for next row iteration
				firstOfPreviousRow = firstOfPreviousRow[2];
			}
			
			// Reference entry points from final row
			corners_[CellIndex.bottomMiddle] = firstOfPreviousRow;
			corners_[CellIndex.bottomRight] = prev;
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
			Cell firstCellOfRow = corners_[5];
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
		
		
		private void spawnSidesFromCorner(CellIndex cornerIndex)
		{
			Cell oldCorner = corners_[cornerIndex];
			
			// Position relative to inside row
			// First convert index to angle
			float rad = Mathf.PI * (float)cornerIndex / 3.0f;
			Vector2 relPos = new Vector2(cellSize_ * Mathf.Sin(rad),
										 cellSize_ * Mathf.Cos(rad));
			
			// Build outwards
			Cell newCorner = new Cell(CellType.Normal);
			// Link inwards
			linkCells(newCorner, oldCorner, cornerIndex-3);
			// Instantiate
			Vector2 oldPos = new Vector2(oldCorner.platform.transform.position.x,
										 oldCorner.platform.transform.position.z);
			instantiateCell(newCorner, oldPos+relPos);
			// Refernce new corner
			corners_[cornerIndex] = newCorner;
			
			// Build anti-clockwise
			Cell prevCell  = newCorner;
			Cell innerCell = oldCorner[cornerIndex-2];
			while (innerCell!=null) {
				Cell c = new Cell(CellType.Normal);
				// Link backwards
				linkCells(c, prevCell, cornerIndex+1);
				// Link inwards
				linkCells(c, innerCell, cornerIndex+3);
				// Link to cell sandwiched between previous two
				linkCells(c, innerCell[cornerIndex+1], cornerIndex+2);
				
				// Instantiate cell relative to inner cell
				Vector2 innerPos = new Vector2(innerCell.platform.transform.position.x,
											   innerCell.platform.transform.position.z);
				instantiateCell(c, innerPos+relPos);
				
				prevCell = c;
				innerCell = innerCell[cornerIndex-2];
			}
			// Reference new anti-clockwise corner
			corners_[cornerIndex-1] = prevCell;
			
			// Build clockwise
			prevCell  = newCorner;
			innerCell = oldCorner[cornerIndex+2];
			while (innerCell!=null) {
				Cell c = new Cell(CellType.Normal);
				// Link backwards
				linkCells(c, prevCell, cornerIndex-1);
				// Link inwards
				linkCells(c, innerCell, cornerIndex-3);
				// Link to cell sandwiched between previous two
				linkCells(c, innerCell[cornerIndex-1], cornerIndex-2);
				
				// Instantiate cell relative to inner cell
				Vector2 innerPos = new Vector2(innerCell.platform.transform.position.x,
											   innerCell.platform.transform.position.z);
				instantiateCell(c, innerPos+relPos);
				
				prevCell = c;
				innerCell = innerCell[cornerIndex+2];
			}
			// Reference new clockwise corner
			corners_[cornerIndex+1] = prevCell;
		}
		
		
		// direction corresponds to corner indices
		public void scroll(CellIndex direction)
		{
			// Spawn two new sides around corner
			spawnSidesFromCorner(direction);
		}
		
		
	}
	
	
}

