using System;
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
	}
	
	
	public class PatternArrayView
	{
		private PatternArrayViewDataSource dataSource_;
		
		private float scrollX_;
		private float scrollY_;
		
		
		private static readonly float cellHeight = 30.0f;
		private static readonly float cellWidth  = cellHeight * Mathf.Sin(Mathf.PI/3.0f);
		
		
		public PatternArrayView(PatternArrayViewDataSource dataSource)
		{
			dataSource_ = dataSource;
		}
		
		
		private static readonly float sbWidth = 15.0f;
		private static readonly Texture2D whiteTex = EditorGUIUtility.whiteTexture;
		public void OnGUI ()
		{
			Rect bounds = GUILayoutUtility.GetRect(0.0f, 0.0f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			
			int rows = dataSource_.numberOfRows   (this);
			int cols = dataSource_.numberOfColumns(this);
			
			// Which cells are viewable?
			Rect drawRegion = new Rect(bounds.x, bounds.y, bounds.width-sbWidth, bounds.height-sbWidth);
			int colStart = (int)Math.Floor(scrollX_);
			int colEnd   = (int)Math.Floor(scrollX_ + (drawRegion.width / cellWidth));
			int rowStart = (int)Math.Floor(scrollY_);
			int rowEnd   = (int)Math.Floor(scrollY_ + (drawRegion.height / cellHeight));
			
			// Scroll displacement
			float xOffset = scrollX_ * cellWidth;
			while (xOffset > cellWidth) xOffset -= cellWidth;
			float yOffset = scrollY_ * cellHeight;
			while (yOffset > cellHeight) yOffset -= cellHeight;
			
			// Draw the cells
			Color oldColor = GUI.color;
			for (int i=0; i<colEnd-colStart; i++) {
				for (int j=0; j<rowEnd-rowStart; j++) {
					Rect cellRect = new Rect(drawRegion.x + ((i+1)*cellWidth) - xOffset,
											 drawRegion.yMax - ((j+1)*cellHeight)+ yOffset + ((colStart+i)%2!=0?cellHeight/2.0f:0.0f),
											 cellWidth,
											 cellHeight);
					CellType type = dataSource_.typeForCell(this, colStart+i, rowStart+j);
					GUI.color = colourForType(type);
					GUI.DrawTexture(cellRect, whiteTex);
				}
			}
			GUI.color = oldColor;
			
			// Scrollbars
			float scrollMaxX_  = (float)cols;
			float scrollMaxY_  = (float)rows;
			float scrollSizeX_ = (bounds.width  - sbWidth) / cellWidth;
			float scrollSizeY_ = (bounds.height - sbWidth) / cellHeight;
			// Prevent scrollbars from being larger than bounds
			if (scrollSizeX_ > scrollMaxX_)
				scrollSizeX_ = scrollMaxX_ = 1.0f;
			if (scrollSizeY_ > scrollMaxY_)
				scrollSizeY_ = scrollMaxY_ = 1.0f;
			Rect scrollXRect = new Rect (bounds.x, bounds.yMax-sbWidth, bounds.width, sbWidth);
			scrollX_ = GUI.HorizontalScrollbar(scrollXRect, scrollX_, scrollSizeX_, 0.0f, scrollMaxX_);
			Rect scrollYRect = new Rect (bounds.xMax-sbWidth, bounds.y, sbWidth, bounds.height-sbWidth);
			scrollY_ = GUI.VerticalScrollbar  (scrollYRect, scrollY_, scrollSizeY_, scrollMaxY_, 0.0f);
			
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

