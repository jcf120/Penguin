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
	
	
	public class PatternArrayView
	{
		private PatternArrayViewDataSource dataSource_;
		
		// GUI members
		private float scrollX_;
		private float scrollY_;
		private float oldScrollMidX_;
		private float oldScrollMidY_;
		private float cellHeight_ = 30.0f;
		private float cellWidth_  = 30.0f * Mathf.Sin(Mathf.PI/3.0f);
		
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
		
		
		private static readonly float sbWidth = 15.0f;
		public void OnGUI ()
		{
			Rect bounds = GUILayoutUtility.GetRect(0.0f, 0.0f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			//GUILayout.BeginArea(bounds);
			
			
			int rows = dataSource_.numberOfRows   (this);
			int cols = dataSource_.numberOfColumns(this);
			
			// Which cells are viewable?
			Rect drawRegion = new Rect(bounds.x, bounds.y, bounds.width-sbWidth, bounds.height-sbWidth);
			int colStart = (int)Math.Floor(scrollX_);
			int colEnd   = (int)Math.Floor(scrollX_ + (drawRegion.width / cellWidth_));
			int rowStart = (int)Math.Floor(scrollY_);
			int rowEnd   = (int)Math.Floor(scrollY_ + (drawRegion.height / cellHeight_));
			// Don't reference beyond data source's bounds
			if (colEnd > cols)
				colEnd = cols;
			if (rowEnd > rows)
				rowEnd = rows;
			
			// Scroll displacement
			float xOffset = scrollX_ * cellWidth_;
			while (xOffset > cellWidth_)  xOffset -= cellWidth_;
			float yOffset = scrollY_ * cellHeight_;
			while (yOffset > cellHeight_) yOffset -= cellHeight_;
			
			// Calc cell vertical offsets
			float oddOffset  = 0.0f;
			float evenOffset = 0.0f;
			if (dataSource_.firstColumnIsEven(this))
				oddOffset  = cellHeight_ / 2.0f;
			else
				evenOffset = cellHeight_ / 2.0f;
			
			
			// Draw the cells
			float texWidth = cellHeight_ / Mathf.Sin(Mathf.PI / 3.0f);
			Color oldColor = GUI.color;
			for (int i=0; i<colEnd-colStart; i++) {
				for (int j=0; j<rowEnd-rowStart; j++) {
					Rect cellRect = new Rect(drawRegion.x + ((i+1)*cellWidth_) - xOffset,
											 drawRegion.yMax - ((j+2)*cellHeight_)+ yOffset + ((colStart+i)%2==0?evenOffset:oddOffset),
											 texWidth,
											 cellHeight_);
					CellType type = dataSource_.typeForCell(this, colStart+i, rowStart+j);
					GUI.color = colourForType(type);
					GUI.DrawTexture(cellRect, hexTex_);
				}
			}
			GUI.color = oldColor;
			
			// Scrollbar max values dependent on data source
			float scrollMaxX  = (float)cols;
			float scrollMaxY  = (float)rows;
			float scrollSizeX = (bounds.width  - sbWidth) / cellWidth_;
			float scrollSizeY = (bounds.height - sbWidth) / cellHeight_;
			
			// Recentre bars on zoom
			scrollX_ = oldScrollMidX_ - (scrollSizeX / 2.0f);
			scrollY_ = oldScrollMidY_ - (scrollSizeY / 2.0f);
			
			// Prevent scrollbars from being larger than bounds
			if (scrollSizeX > scrollMaxX) {
				scrollSizeX = scrollMaxX = 1.0f;
				scrollX_ = 0.0f;
			}
			if (scrollSizeY > scrollMaxY) {
				scrollSizeY = scrollMaxY = 1.0f;
				scrollY_ = 1.0f;
			}
			
			// Draw the scrollbars
			Rect scrollXRect = new Rect (bounds.x, bounds.yMax-sbWidth, bounds.width, sbWidth);
			scrollX_ = GUI.HorizontalScrollbar(scrollXRect, scrollX_, scrollSizeX, 0.0f, scrollMaxX);
			Rect scrollYRect = new Rect (bounds.xMax-sbWidth, bounds.y, sbWidth, bounds.height-sbWidth);
			scrollY_ = GUI.VerticalScrollbar  (scrollYRect, scrollY_, scrollSizeY, scrollMaxY, 0.0f);
			
			
			
			// Zoom detail GUI
			Rect sliderRect = new Rect(bounds.x, bounds.y, sbWidth, 50.0f);
			cellHeight_ = GUI.VerticalSlider(sliderRect, cellHeight_, 40.0f, 5.0f);
			cellWidth_  = cellHeight_ * Mathf.Sin(Mathf.PI/3.0f);
			// Values for keeping scrollbar centred on zoom
			oldScrollMidX_ = scrollX_ + (scrollSizeX / 2.0f);
			oldScrollMidY_ = scrollY_ + (scrollSizeY / 2.0f);
			
			//GUILayout.EndArea();
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

