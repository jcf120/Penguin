using System;
using System.Collections.Generic;

namespace Penguin
{
	[Serializable]
	public class SinePattern : CellPattern
	{
		public float amplitude;
		public float wavelength;
		public int   width;
		
		public override Dictionary<string, object> packDict ()
		{
			Dictionary<string, object> data = base.packDict ();
			
			data["amplitude" ] = amplitude;
			data["wavelength"] = wavelength;
			data["width"     ] = width;
			
			return data;
		}
		
		
		public override void unpackDict (Dictionary<string, object> data)
		{
			base.unpackDict (data);
			
			amplitude  = (float)data["amplitude"];
			wavelength = (float)data["wavelength"];
			width      = Convert.ToInt32(data["width"]);
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

