using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Penguin;

namespace LevelEditor
{
	public interface PatternArrayViewDataSource
	{
		int      numberOfRows(PatternArrayView view);
		int      numberOfColumns(PatternArrayView view);
		CellType typeForCell(PatternArrayView view, int col, int row);
		bool     firstColumnIsEven(PatternArrayView view);
	}
	
	
	public interface PatternArrayViewResponder
	{
		void cellLeftClicked (PatternArrayView view, int col, int row);
		void cellRightClicked(PatternArrayView view, int col, int row);
	}
	
	
	public class PatternArrayView
	{
		private PatternArrayViewDataSource dataSource_;
		private PatternArrayViewResponder  responder_;
		public  PatternArrayViewResponder  responder {set{responder_=value;}}
		
		// GUI members
		private float scrollCentreX_;
		private float scrollCentreY_;
		private float cellHeight_ = 30.0f;
		private float cellWidth_  = 30.0f * Mathf.Sin(Mathf.PI/3.0f);
		
		// Scrollbar thickness
		private static readonly float sbWidth = 15.0f;
		
		private Texture2D hexTex_;
		
		
		public PatternArrayView(PatternArrayViewDataSource dataSource)
		{
			dataSource_ = dataSource;
			
			// Load cell texture
			hexTex_ = new Texture2D(0,0);
			FileStream fs = new FileStream("Assets/Editor/Hexagon.png", FileMode.Open, FileAccess.Read);
			byte[] imageData = new byte[fs.Length];
			fs.Read(imageData, 0, (int) fs.Length);
			hexTex_.LoadImage(imageData);
			
		}
		public void OnGUI ()
		{
			Rect bounds = GUILayoutUtility.GetRect(0.0f,
												   0.0f,
												   GUILayout.ExpandWidth(true),
												   GUILayout.ExpandHeight(true));
			// Push coordinates and clipping space
			GUI.BeginGroup(bounds);
			bounds.x = bounds.y = 0.0f;
			
			// Size of data source
			int rows = dataSource_.numberOfRows   (this);
			int cols = dataSource_.numberOfColumns(this);
			
			// Zoom slider draw later, but rect need for ignoring input
			Rect zoomRect = new Rect(bounds.x + 10.0f,
									 bounds.y + 10.0f,
									 15.0f,
									 50.0f);
			
			// Draw the cells and handle mouse event
			Rect cellRegion    = bounds;
			cellRegion.width  -= sbWidth;
			cellRegion.height -= sbWidth;
			drawCellsAndDetectMouse(cellRegion, zoomRect, cols, rows);
			
			// Draw zoom ontop of cells
			drawZoomSlider(zoomRect);
			
			// Scrollbars
			if (cols > 0 && rows > 0)
				drawScrollbars(bounds, cols, rows);
			
			// Pop coordinates and clipping space
			GUI.EndGroup();
		}
		
		
		private float positiveDecimal(float x)
		{
			while (x < 0.0f) x += 1.0f;
			return x % 1.0f;
		}
		
		
		private void drawCellsAndDetectMouse(Rect region, Rect ignoreRegion, int colMax, int rowMax)
		{
			// Mouse input
			Event evt = Event.current;
			bool checkMouse = responder_ != null;
			// Only interested in mouse down
			if (evt.type != EventType.MouseDown && evt.type != EventType.MouseDrag)
				checkMouse = false;
			// Check input falls inside region
			Vector2 mousePos = evt.mousePosition;
			if (!region.Contains(mousePos))
				checkMouse = false;
			// Ignore event if inside ignore region for sliderbar
			if (ignoreRegion.Contains(mousePos))
				checkMouse = false;
			
			// Viewable range
			float halfViewableWidth  = (region.width  / cellWidth_ ) / 2.0f;
			float halfViewableHeight = (region.height / cellHeight_) / 2.0f;
			int colStart = (int)Math.Floor  (scrollCentreX_) - (int)Math.Ceiling(halfViewableWidth);
			int rowStart = (int)Math.Floor  (scrollCentreY_) - (int)Math.Ceiling(halfViewableHeight);
			int colEnd   = (int)Math.Ceiling(scrollCentreX_) + (int)Math.Ceiling(halfViewableWidth);
			int rowEnd   = (int)Math.Ceiling(scrollCentreY_) + (int)Math.Ceiling(halfViewableHeight);
			
			// Scroll displacement is the scroll's positive decimal
			float colOffset = positiveDecimal(scrollCentreX_) + positiveDecimal(halfViewableWidth);
			float rowOffset = positiveDecimal(scrollCentreY_) + positiveDecimal(halfViewableHeight);
			
			// Calc cell vertical offsets for alternating rows
			float oddOffset  = 0.0f;
			float evenOffset = 0.0f;
			if (dataSource_.firstColumnIsEven(this))
				oddOffset  = cellHeight_ / 2.0f;
			else
				evenOffset = cellHeight_ / 2.0f;
			
			
			float texWidth = cellHeight_ / Mathf.Sin(Mathf.PI / 3.0f);
			
			Color oldColor = GUI.color;
			
			for (int i=colStart; i<colEnd; i++) {
				for (int j=rowStart; j<rowEnd; j++) {
					
					// Only draw if inside data range
					if (   i>=0 && i<colMax
						&& j>=0 && j<rowMax) {
						
						CellType type = dataSource_.typeForCell(this, i, j);
						GUI.color = colourForType(type);
						
						float x = region.x    + ((i - 0.5f - colStart - colOffset) * cellWidth_);
						float y = region.yMax - ((j + 1.0f - rowStart - rowOffset) * cellHeight_);
						y += i % 2 == 0 ? evenOffset : oddOffset;
						
						Rect cellRect = new Rect(x, y, texWidth, cellHeight_);
						GUI.DrawTexture(cellRect, hexTex_);
						
						// Check mouse input
						if (checkMouse && cellRect.Contains(mousePos)) {
							checkMouse = false;
							if      (evt.button == 0)
								responder_.cellLeftClicked (this, i, j);
							else if (evt.button == 1)
								responder_.cellRightClicked(this, i, j);
							evt.Use();
						}
					}
				}
			}
			
			GUI.color = oldColor;
		}
		
		
		private void drawScrollbars(Rect region, int colMax, int rowMax)
		{
			// Scrollbar max values dependent on data source
			// Allow scrolling slightly beyond data set
			float dataCentreX     = (colMax / 2.0f);
			float dataCentreY     = (rowMax / 2.0f);
			float scrollHalfMaxX  = dataCentreX + 1.0f;
			float scrollHalfMaxY  = dataCentreY + 1.0f;
			float scrollSizeX     = (region.width  - sbWidth) / cellWidth_;
			float scrollSizeY     = (region.height - sbWidth) / cellHeight_;
			
			// ScrollMax must at least as big as ScrollSize
			if (scrollHalfMaxX * 2.0f < scrollSizeX) {
				scrollHalfMaxX = scrollSizeX / 2.0f;
				scrollCentreX_ = dataCentreX;
			}
			if (scrollHalfMaxY * 2.0f < scrollSizeY) {
				scrollHalfMaxY = scrollSizeY / 2.0f;
				scrollCentreY_ = dataCentreY;
			}
			
			// GUI uses left/ bottom of scrollbar
			float scrollX = scrollCentreX_ - (scrollSizeX / 2.0f);
			float scrollY = scrollCentreY_ - (scrollSizeY / 2.0f);
			
			Rect scrollXRect = new Rect (region.x,
										 region.yMax - sbWidth,
										 region.xMax,
										 sbWidth);
			Rect scrollYRect = new Rect (region.xMax - sbWidth,
										 region.x,
										 sbWidth,
										 region.yMax - sbWidth);
			
			scrollX = GUI.HorizontalScrollbar(scrollXRect,
											  scrollX,
											  scrollSizeX,
											  dataCentreX - scrollHalfMaxX,
											  dataCentreX + scrollHalfMaxX);
			scrollY = GUI.VerticalScrollbar  (scrollYRect,
											  scrollY,
											  scrollSizeY,
											  dataCentreY + scrollHalfMaxY,
											  dataCentreY - scrollHalfMaxY);
			
			// Update class scroll memebers
			scrollCentreX_ = scrollX + (scrollSizeX / 2.0f);
			scrollCentreY_ = scrollY + (scrollSizeY / 2.0f);
		}
		
		
		private void drawZoomSlider(Rect region)
		{
			float oldCellHeight = cellHeight_;
			cellHeight_ = GUI.VerticalSlider(region, cellHeight_, 40.0f, 5.0f);
			// Zoom changed
			if (oldCellHeight != cellHeight_)
				cellWidth_  = cellHeight_ * Mathf.Sin(Mathf.PI/3.0f);
		}
		
		
		private void detectMouseInput(Rect region)
		{
			Event evt = Event.current;
			
			// Only interested in mouse down event
			if (evt.type != EventType.MouseDown)
				return;
			
			Vector2 mousePos = evt.mousePosition;
			
			// Ignore event if outside draw region
			if (!region.Contains(mousePos))
				return;
			
			evt.Use();
			
			// Invert y and move to region's frame
			mousePos.x -= region.x;
			mousePos.y =  region.yMax - mousePos.y;
			
			// Scale and account for scroll offset
			mousePos   /= cellHeight_;
			mousePos.x += scrollCentreX_ - 0.5f - ((region.width  / cellWidth_ ) / 2.0f);
			mousePos.y += scrollCentreY_ - 0.5f - ((region.height / cellHeight_) / 2.0f);
			
			// Consider alternating column displacment
			if (!dataSource_.firstColumnIsEven(this))
				mousePos.y += 0.5f;
			
			// Convert to hexagon coordinates
			PatternCoordinate pc = new PatternCoordinate(CellVector.fromVector2(mousePos));
			Debug.Log(pc);
		}
		
		
		private Color colourForType(CellType type)
		{
			switch(type)
			{
			case CellType.Undefined:
				return Color.magenta;
			case CellType.Empty:
				return Color.black;
			case CellType.Normal:
				return Color.grey;
			default:
				Debug.LogError("PatternArrayView handed unrecognised CellType.");
				return Color.blue;
			}
		}
		
	}
}

