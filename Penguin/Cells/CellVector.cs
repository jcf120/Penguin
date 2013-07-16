using System;

namespace Penguin
{
	public struct CellVector
	{
		private int x_; // represents up and right
		private int y_; // represents up
		public int x {get{return x_;}set{x_=value;}}
		public int y {get{return y_;}set{y_=value;}}
		
		
		public CellVector(int x, int y)
		{
			x_ = x;
			y_ = y;
		}
		
		
		public CellVector(CellIndex dir, int mag)
		{
			if        (dir == CellIndex.topMiddle) {
				x_ = 0;
				y_ = mag;
			} else if (dir == CellIndex.topRight) {
				x_ = mag;
				y_ = 0;
			} else if (dir == CellIndex.bottomRight) {
				x_ = mag;
				y_ = mag;
			} else if (dir == CellIndex.bottomMiddle) {
				x_ = 0;
				y_ = -mag;
			} else if (dir == CellIndex.bottomLeft) {
				x_ = -mag;
				y_ = 0;
			} else if (dir == CellIndex.topLeft) {
				x_ = -mag;
				y_ = mag;
			} else { // Case for invalid direction
				x_ = 0;
				y_ = 0;
			}
		}
		
		
		public CellVector rotated(CellIndex ang)
		{
			CellVector result = new CellVector(0, 0);
			if        (ang == CellIndex.topMiddle) {
				// No change
			} else if (ang == CellIndex.topRight) {
				result.x = x_ + y_;
				result.y = -x_;
			} else if (ang == CellIndex.bottomRight) {
				result.x = y_;
				result.y = -x_ - y_;
			} else if (ang == CellIndex.bottomMiddle) {
				result.x = -x_;
				result.y = -y_;
			} else if (ang == CellIndex.bottomLeft) {
				result.x = -x_ - y_;
				result.y = x_;
			} else if (ang == CellIndex.topLeft) {
				result.x = -y_;
				result.y = x_ + y_;
			}
			return result;
		}
		
		
		public static CellVector operator+ (CellVector lhs, CellVector rhs)
		{
			lhs.x += rhs.x;
			lhs.y += rhs.y;
			return lhs;
		}
		
		
		public static CellVector operator- (CellVector lhs, CellVector rhs)
		{
			lhs.x -= rhs.x;
			lhs.y -= rhs.y;
			return lhs;
		}
	}
}

