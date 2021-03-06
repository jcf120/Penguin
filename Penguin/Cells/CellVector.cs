using System;
using UnityEngine;

namespace Penguin
{
	public struct CellVector
	{
		private int i_; // represents up and right
		private int j_; // represents up
		public int i {get{return i_;}set{i_=value;}}
		public int j {get{return j_;}set{j_=value;}}
		
		
		public CellVector(int i, int j)
		{
			i_ = i;
			j_ = j;
		}
		
		
		public CellVector(CellIndex dir, int mag)
		{
			if        (dir == CellIndex.topMiddle) {
				i_ = 0;
				j_ = mag;
			} else if (dir == CellIndex.topRight) {
				i_ = mag;
				j_ = 0;
			} else if (dir == CellIndex.bottomRight) {
				i_ = mag;
				j_ = -mag;
			} else if (dir == CellIndex.bottomMiddle) {
				i_ = 0;
				j_ = -mag;
			} else if (dir == CellIndex.bottomLeft) {
				i_ = -mag;
				j_ = 0;
			} else if (dir == CellIndex.topLeft) {
				i_ = -mag;
				j_ = mag;
			} else {
				i_ = 0;
				j_ = 0;
				Debug.LogError("Instantiating CellVector from invalid CellIndex");
			}
		}
		
		
		public CellVector rotated(CellIndex ang)
		{
			CellVector result = new CellVector(0, 0);
			if        (ang == CellIndex.topMiddle) {
				result.i = i_;
				result.j = j_;
			} else if (ang == CellIndex.topRight) {
				result.i = i_ + j_;
				result.j = -i_;
			} else if (ang == CellIndex.bottomRight) {
				result.i = j_;
				result.j = -i_ - j_;
			} else if (ang == CellIndex.bottomMiddle) {
				result.i = -i_;
				result.j = -j_;
			} else if (ang == CellIndex.bottomLeft) {
				result.i = -i_ - j_;
				result.j = i_;
			} else if (ang == CellIndex.topLeft) {
				result.i = -j_;
				result.j = i_ + j_;
			} else {
				Debug.LogError("CellVector can't rotate by invalid angle: "+ang);
			}
			return result;
		}
		
		
		private static readonly float sin60 = Mathf.Sin(Mathf.PI/3.0f);
		private static readonly float cos60 = Mathf.Cos(Mathf.PI/3.0f);
		public Vector2 vector2(float unitLength)
		{
			return new Vector2(unitLength*i_*sin60,
							   unitLength*(j_ + i_*cos60));
		}
		
		
		public static CellVector operator+ (CellVector lhs, CellVector rhs)
		{
			lhs.i += rhs.i;
			lhs.j += rhs.j;
			return lhs;
		}
		
		
		public static CellVector operator- (CellVector lhs, CellVector rhs)
		{
			lhs.i -= rhs.i;
			lhs.j -= rhs.j;
			return lhs;
		}
		
		
		public static CellVector operator* (int factor, CellVector cv)
		{
			cv.i *= factor;
			cv.j *= factor;
			return cv;
		}
		
		
		public static int jFromInts(int x, int y)
		{
			return (int)Math.Floor((y-x+1)/3.0f);
		}
		
		
		public static int iFromInts(int x, int y)
		{
			return (int)Math.Floor((x-y+1)/3.0f);
		}
		
		
		// Quantise Vector2
		public static CellVector fromVector2(Vector2 vec2)
		{
			CellVector cv = new CellVector(0, 0);
			
			// Affine transform for finding i
			float x = (vec2.x * 2.0f * sin60) + (vec2.y * ((2.0f * sin60 * sin60) - 0.5f));
			float y = vec2.y - (2.0f * vec2.x * sin60);
			
			cv.i = (int)Math.Floor(((int)Math.Floor(x) - (int)Math.Floor(y) + 1) / 3.0f);
			
			// Affine transform for finding j
			x = (2.0f * vec2.x * sin60) - vec2.y;
			y = vec2.y * 2.0f;
			
			cv.j = (int)Math.Floor(((int)Math.Floor(y) - (int)Math.Floor(x) + 1) / 3.0f);
			
			return cv;
		}
		
		
		// Logging
		public override string ToString()
		{
			return "CellVector("+i+","+j+")";
		}
		
	}
}

