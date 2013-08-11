using System;
using System.Collections.Generic;

namespace Penguin
{
	[Serializable]
	public class SingleTypePattern : CellPattern
	{
		public CellType cellType;
		
		
		public override Dictionary<string,object> packDict()
		{
			Dictionary<string,object> data = base.packDict();
			
			data["cellType"] = cellType;
			
			return data;
		}
		
		
		public override void unpackDict (Dictionary<string, object> data)
		{
			base.unpackDict (data);
			
			cellType = (CellType)Enum.Parse(typeof(CellType), (string)data["cellType"]);
		}
		
		
		public override CellType typeAtCoordinate(PatternCoordinate coor)
		{
			return cellType;
		}
	}
}

