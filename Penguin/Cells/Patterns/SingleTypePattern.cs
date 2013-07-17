using System;

namespace Penguin
{
	public class SingleTypePattern : CellPattern
	{
		private CellType type_;
		
		public SingleTypePattern (CellType type)
		{
			type_ = type;
		}
		
		public override CellType typeAtCoordinate(PatternCoordinate coor)
		{
			return type_;
		}
	}
}

