using System;
using UnityEngine;

namespace Penguin
{
	public class FreePattern : CellPattern
	{
		public string storeName; // used by PatternArray controller to match store on unpacking
		public FreePatternStore store;
		
		
		public void OnEnable ()
		{
			if (store == null)
				store = new FreePatternStore();
		}
		
		
		public override CellType typeAtCoordinate (PatternCoordinate coor)
		{
			// Convert to store's frame and check bounds
			coor.col -= colsLeft;
			if (   coor.col >= 0 && coor.col < store.width
				&& coor.row >= 0 && coor.row < store.height) {
				return store.values[coor.col,coor.row];
			}
			else {
				Debug.LogError("FreePattern queried beyond bounds. Defaulting to CellType.Empty");
				return CellType.Empty;
			}
			
		}
	}
}

