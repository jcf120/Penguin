using System;

namespace Penguin
{
	[Serializable]
	public class SinePattern : CellPattern
	{
		public float amplitude;
		public float wavelength;
		public int   width;
		
		public SinePattern ()
		{
			
		}
		
		
		public override CellType typeAtCoordinate (PatternCoordinate coor)
		{
			// wave in a function row
			int sinOfRow = (int)Math.Floor(amplitude * Math.Sin(2.0f * Math.PI * coor.row / wavelength));
			
			// check column falls within thickness
			if (coor.col > sinOfRow-width && coor.col < sinOfRow+width)
				return CellType.Normal;
			else
				return CellType.Empty;
		}
	}
}

