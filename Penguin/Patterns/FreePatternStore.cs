using System;
using UnityEngine;
using Penguin;

namespace Penguin
{
	public class FreePatternStore : ScriptableObject
	{
		public int width;
		public int height;
		public CellType[][] values;
	}
}

