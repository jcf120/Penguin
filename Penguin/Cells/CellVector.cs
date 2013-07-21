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
		
		
		// Quantise Vector2
		public static CellVector fromVector2(Vector2 vec2)
		{
			CellVector cv = new CellVector(0, 0);
			
			// convert x to i
			cv.i = (int)Math.Round(vec2.x / sin60, 0);
			
			// convert x&y to j
			cv.j = (int)Math.Round(vec2.y - (vec2.x / cos60), 0);
			
			return cv;
		}
		
	}
}

