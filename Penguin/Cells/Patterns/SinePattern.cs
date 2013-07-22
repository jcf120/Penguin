using System;

namespace Penguin
{
	public class SinePattern : CellPattern
	{
		private float amplitude_;
		private float wavelength_;
		private int   width_;
		
		public SinePattern (float amplitude, float wavelength, int width)
		{
			amplitude_  = amplitude;
			wavelength_ = wavelength;
			width_      = width;
		}
		
		
		public override CellType typeAtCoordinate (PatternCoordinate coor)
		{
			// wave in a function row
			int sinOfRow = (int)Math.Floor(amplitude_ * Math.Sin(2.0f * Math.PI * coor.row / wavelength_));
			
			// check column falls within thickness
			if (coor.col > sinOfRow-width_ && coor.col < sinOfRow+width_)
				return CellType.Normal;
			else
				return CellType.Empty;
		}
	}
}

