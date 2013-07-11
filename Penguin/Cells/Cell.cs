using System;
using UnityEngine;

namespace Penguin
{
	// This is a class rather than a struct so
	// that it's treated as a reference rather
	// than a value
	public class Cell
	{
		
		// Access to surrounding cells
		// Indices defined clockwise from top
		// 
		//      --0--
		//    /       \
		//   5         1
		//  /           \
		//  \           /
		//   4         2
		//    \       /
		//      --3--
		private Cell[] neighbours_ = {null, null, null, null, null, null};
		// Access via subscript operator
		public Cell this[int index]
		{
			get {return neighbours_[index];}
			set {neighbours_[index] = value;}
		}
		
		// Value of cell
		public CellType type = CellType.Undefined;
		
		// Reference to in-game platform object
		public GameObject platform;
		
		
		// Type Constructor
		public Cell(CellType cellType)
		{
			type = cellType;
		}
		
	}
}

