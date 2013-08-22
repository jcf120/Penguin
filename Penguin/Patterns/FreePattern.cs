using System;
using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
	public class FreePattern : CellPattern
	{
		public string storeName = "unassigned"; // used by PatternArray controller to match store on unpacking
		public FreePatternStore store;
		
		
		public override System.Collections.Generic.Dictionary<string, object> packDict ()
		{
			Dictionary<string, object> data = base.packDict ();
			data["storeName"] = storeName;
			return data;
		}
		
		
		public override void unpackDict (System.Collections.Generic.Dictionary<string, object> data)
		{
			base.unpackDict (data);
			storeName = data["storeName"] as string;
		}
		
		
		public override CellType typeAtCoordinate (PatternCoordinate coor)
		{
			if (store == null) {
				return CellType.Undefined;
			}
			
			// Convert to store's frame and check bounds
			coor.col += colsLeft;
			if (   coor.col >= 0 && coor.col < store.width
				&& coor.row >= 0 && coor.row < store.height) {
				return store[coor.col,coor.row];
			}
			else {
				return CellType.Undefined;
			}
			
		}
	}
}

