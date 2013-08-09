using System;

namespace Penguin
{
	[Serializable]
	public class SingleTypePattern : CellPattern
	{
		public CellType type;
		
		public SingleTypePattern ()
		{
			
		}
		
		public override CellType typeAtCoordinate(PatternCoordinate coor)
		{
			return type;
		}
	}
}

