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
		
		// Informtion for CellMap to build with 
		private CellPattern currentPattern_;
		private CellIndex   patternDirection_;
		// Displacement of CellMap's centre from current CellPattern's origin
		CellVector patternPosition_;
		
		
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
		private Vector2 centre_;
		public Vector2 centre {get{return centre_;}}
		
		
		public CellMap (int radius,
						float cellSize,
						Vector2 centre,
						Dictionary<CellType, GameObject> platfromDict)
		{
			if (radius<1) Debug.LogError("CellMap cannot have radius below 1");
			radius_       = radius;
			cellSize_     = cellSize;
			centre_		  = centre;
			platformDict_ = platfromDict;
			buildInitialCells(CellType.Normal);
			
			// Setup default pattern
			currentPattern_   = new SingleTypePattern(CellType.Normal);
			patternDirection_ = new CellIndex(0);
			patternPosition_  = new CellVector(patternDirection_, radius);
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
		private void unlinkCells(Cell c1, Cell c2, CellIndex c1index)
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
			for (int i=0; i<radius_; i++) {
				Cell c = new Cell(type);
				// Link bottom left corner
				linkCells(c, prev, CellIndex.bottomLeft);
				prev = c;
			}
			
			// Reference top middle entry point
			corners_[CellIndex.topMiddle] = prev;
			
			// Build rows underneath until we reach the top-right corner
			Cell firstOfPreviousRow = corners_[CellIndex.topLeft];
			for (int i=0; i<radius_; i++) {
				// First cell of row
				prev = new Cell(type);
				// Link to cell above
				Cell abovePrev = firstOfPreviousRow;
				linkCells(prev, abovePrev, CellIndex.topMiddle);
				
				// Remaining cells in row
				for (int j=0; j<radius_+i+1; j++) {
					
					Cell c = new Cell(type);
					// Link bottom left
					linkCells(c, prev, CellIndex.bottomLeft);
					// Link top left
					linkCells(c, abovePrev, CellIndex.topLeft);
					// Link above, unless last cell in row
					if (j<radius_+i) {
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
			for (int i=radius_; i>0; i--) {
				
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
		
		
		// Destroys contents but doesn't remove from map
		private void destroyCell(Cell cell)
		{
			if (cell.platform!=null) {
				GameObject.Destroy(cell.platform);
				cell.type = CellType.Empty;
			} else {
				Debug.LogError("Cell's platform of type "+cell.type+" already null");
			}
		}
		
		
		// Destroy and delete cell from map
		private void deleteCell(Cell cell)
		{
			// Unlink from neighbours
			for (int i=0; i<6; i++)
				if (cell[i]!=null) {
				CellIndex index = new CellIndex(i);
				unlinkCells(cell, cell[i], index);
			}
				
			// Destroy if of non-empty type
			if (cell.type != CellType.Empty && cell.type != CellType.Undefined)
				destroyCell(cell);
		}
		
		
		private bool initialCellsAreInstantiated_;
		public void instantiateInitialCells()
		{
			if (initialCellsAreInstantiated_) Debug.LogError("Cells have been instantiated once already");
			initialCellsAreInstantiated_ = true;
			
			// Cell traversal vectors
			Vector2 rightUp = new Vector2(cellSize_*Mathf.Sin(Mathf.PI/3.0f),
												cellSize_*Mathf.Cos(Mathf.PI/3.0f));
			Vector2 down = -cellSize_ * Vector2.up;
			
			Vector2 firstPosOfRow = (radius_ * (-rightUp-down)) + centre_;
			Cell firstCellOfRow = corners_[CellIndex.topLeft];
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
		
		
		// Look up cell from pattern
		private Cell cellForVector(CellVector mapVec, CellIndex spawnDir)
		{
			// Translate and rotate into Pattern's frame
			CellVector relPos = mapVec - patternPosition_;
			relPos = relPos.rotated(-patternDirection_);
			// Convert to wavy coordinates
			PatternCoordinate pc = new PatternCoordinate(0,0) + relPos;
			
			return new Cell(currentPattern_.typeAtCoordinate(pc));
		}
		
		
		private void spawnSidesFromCorner(CellIndex cornerIndex)
		{
			Cell oldCorner = corners_[cornerIndex];
			
			// New corner position relative to centre
			CellVector newCornerPos = new CellVector(cornerIndex, radius_+1);
			
			// Build outwards
			Cell newCorner = cellForVector(newCornerPos, cornerIndex);
			// Link inwards
			linkCells(newCorner, oldCorner, cornerIndex-3);
			// Instantiate
			instantiateCell(newCorner, centre_ + newCornerPos.vector2(cellSize_));
			// Reference new corner
			corners_[cornerIndex] = newCorner;
			
			// Build anti-clockwise
			Cell prevCell  = newCorner;
			Cell innerCell = oldCorner[cornerIndex-2];
			CellVector stepVec = new CellVector(cornerIndex-2, 1);
			CellVector relPos = newCornerPos;
			while (innerCell!=null) {
				// New cells position relative to centre
				relPos += stepVec;
				
				Cell c = cellForVector(relPos, cornerIndex);
				// Link backwards
				linkCells(c, prevCell, cornerIndex+1);
				// Link inwards
				linkCells(c, innerCell, cornerIndex+3);
				// Link to cell sandwiched between previous two
				linkCells(c, innerCell[cornerIndex+1], cornerIndex+2);
				instantiateCell(c, centre_ + relPos.vector2(cellSize_));
				
				prevCell = c;
				innerCell = innerCell[cornerIndex-2];
			}
			// Reference new anti-clockwise corner
			corners_[cornerIndex-1] = prevCell;
			
			// Build clockwise
			prevCell  = newCorner;
			innerCell = oldCorner[cornerIndex+2];
			stepVec = new CellVector(cornerIndex+2, 1);
			relPos = newCornerPos;
			while (innerCell!=null) {
				// New cells position relative to centre
				relPos += stepVec;
				
				Cell c = cellForVector(relPos, cornerIndex);
				// Link backwards
				linkCells(c, prevCell, cornerIndex-1);
				// Link inwards
				linkCells(c, innerCell, cornerIndex-3);
				// Link to cell sandwiched between previous two
				linkCells(c, innerCell[cornerIndex-1], cornerIndex-2);
				
				
				instantiateCell(c, centre_+relPos.vector2(cellSize_));
				
				prevCell = c;
				innerCell = innerCell[cornerIndex+2];
			}
			// Reference new clockwise corner
			corners_[cornerIndex+1] = prevCell;
		}
		
		
		private void deleteSidesFromCorner(CellIndex cornerIndex)
		{
			// Record old corner
			Cell oldCorner = corners_[cornerIndex];
			
			// Update this corner and the two either side
			corners_[cornerIndex  ] = corners_[cornerIndex  ][cornerIndex-3];
			corners_[cornerIndex-1] = corners_[cornerIndex-1][cornerIndex-3];
			corners_[cornerIndex+1] = corners_[cornerIndex+1][cornerIndex-3];
			
			// delete anti-clockwise
			Cell next = oldCorner[cornerIndex-2];
			while (next!=null) {
				Cell c = next;
				next = next[cornerIndex-2];
				deleteCell(c);
			}
			
			// delete clockwise
			next = oldCorner[cornerIndex+2];
			while (next!=null) {
				Cell c = next;
				next = next[cornerIndex+2];
				deleteCell(c);
			}
			
			// Delete old corner
			deleteCell(oldCorner);
		}
		
		
		// direction corresponds to corner indices
		public void scroll(CellIndex direction)
		{
			// Spawn two new sides around corner
			spawnSidesFromCorner(direction);
			// Delete opposite sides
			deleteSidesFromCorner(direction-3);
			// Update centre
			centre_ += new CellVector(direction, 1).vector2(cellSize_);
			// Update Pattern's relative position
			patternPosition_ -= new CellVector(direction, 1);
			CellVector cv = new CellVector(direction-3, 1);
		}
		
		
		private void repositionCell(Cell cell, Vector2 position)
		{
			if (cell.platform == null) {
				Debug.LogError("Attempted to reposition null platform");
				return;
			}
			Vector3 pos = cell.platform.transform.position;
			pos.x = position.x;
			pos.z = position.y;
			cell.platform.transform.position = pos;
		}
		
		
		// Uses new centre and cell's relative positions to rearrange platforms
		public void repositionAroundCentre(Vector2 centre)
		{
			centre_ = centre;
			
			// Cell traversal vectors
			Vector2 rightUp = new Vector2(cellSize_*Mathf.Sin(Mathf.PI/3.0f),
												cellSize_*Mathf.Cos(Mathf.PI/3.0f));
			Vector2 down = -cellSize_ * Vector2.up;
			
			Vector2 firstPosOfRow = (radius_ * (-rightUp-down)) + centre_;
			Cell firstCellOfRow = corners_[CellIndex.topLeft];
			while (firstCellOfRow!=null) {
				
				Cell    c = firstCellOfRow;
				Vector2 v = firstPosOfRow;
				while (c!=null) {
					// Check cell has contents
					if (c.type != CellType.Undefined && c.type != CellType.Empty)
						repositionCell(c, v);
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
		
		
		// Prepare CellMap to change pattern
		public void changePattern(CellPattern pattern, CellIndex direction)
		{
			// Expand this later to deal with transitions
			patternDirection_ = direction;
			patternPosition_  = new CellVector(direction, radius_);
			currentPattern_   = pattern;
		}
		
		
	}
	
	
}

