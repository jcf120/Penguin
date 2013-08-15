using System;
using UnityEngine;

namespace Penguin
{
	public class FreePattern : CellPattern
	{
		public string storeName; // used by PatternArray controller to match store on unpacking
		private FreePatternStore store_;
		
		
		public void OnEnable ()
		{
			if (store_ == null)
				store_ = new FreePatternStore();
		}
		
		
		public override CellType typeAtCoordinate (PatternCoordinate coor)
		{
			// Convert to store's frame and check bounds
			coor.col -= colsLeft;
			if (coor.col < store_.width && coor.row < store_.height) {
				return store_.values[coor.col,coor.row];
			}
			else {
				Debug.LogError("FreePattern queried beyond bounds. Defaulting to CellType.Empty");
				return CellType.Empty;
			}
			
		}
	}
}

