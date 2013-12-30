//------------------------------------------------------------------------------
// File:	TorusArray.cs
// Author:	Jocelyn Clifford-Frith
// Date:	27th December 2013
//
// Description:
// A generic 2-dimesional container that can subscripted as if it were a 2D
// array. The content has the ability to "scroll" both vertically and
// horizontally by using an offset on the origin. This doesn't actually move
// any elements, but creates the illusion by wrapping index values the current
// origin offset.
// 
// coordinates are defined left-to-right and top-to-bottom:
//   0  1  2
// 0 +------
// 0 |
// 1 |
// 2 |
//------------------------------------------------------------------------------

using System;
namespace Penguin
{
	//--------------------------------------------------------------------------
	public class TorusArray<T>
	{


		//----------------------------------------------------------------------
		// Private members
		T[] 			m_values;
		uint 			m_originX;
		uint 			m_originY;
		readonly uint 	m_sizeX;
		readonly uint 	m_sizeY;
		
		
		//======================================================================
		// Public Methods
		//======================================================================

		
		//----------------------------------------------------------------------
		public TorusArray (uint x, uint y)
		{
			// Assert that x and y are non-zero and positive
			if (x < 1)
			{
				throw new ArgumentException ("x argument must be non-zero and positive",
				                             "x");
			}
			if (y < 1)
			{
				throw new ArgumentException ("y argument must be non-zero and positive",
				                             "y");
			}

			// Allocate the elements
			m_values = new T[x * y];

			// Note the container's size
			m_sizeX = x;
			m_sizeY = y;

			// Origin defaults to (0, 0)
			m_originX = 0;
			m_originY = 0;
		}

		
		//----------------------------------------------------------------------
		// Set the array's orgin relative to it's current position
		public void Translate (int x, int y)
		{
			m_originX += (uint)x;
			m_originY += (uint)y;

			// Wrap the values so they fall within the array's bounds
			m_originX = WrapIndex((int)m_originX, m_sizeX);
			m_originY = WrapIndex((int)m_originY, m_sizeY);
		}

		
		//----------------------------------------------------------------------
		// Convenience methods
		public void StepLeft()	{ Translate(-1, 0); }
		public void StepRight()	{ Translate( 1, 0); }
		public void StepUp()	{ Translate( 0, 1); }
		public void StepDown()	{ Translate( 0,-1); }

		
		//----------------------------------------------------------------------
		public T this[uint x, uint y]
		{
			get
			{
				// Check within array bounds
				if (x < 0 || x >= m_sizeX)
				{
					throw new ArgumentException ("x argument is outside TorusArray's bounds",
					                             "x");
				}
				if (y < 0 || y >= m_sizeY)
				{
					throw new ArgumentException ("y argument is outside TorusArray's bounds",
					                             "y");
				}
				
				return m_values[ LinearIndex(x, y) ];
			}

			set
			{
				// Check within array bounds
				if (x < 0 || x >= m_sizeX)
				{
					throw new ArgumentException ("x argument is outside TorusArray's bounds",
					                             "x");
				}
				if (y < 0 || y >= m_sizeY)
				{
					throw new ArgumentException ("y argument is outside TorusArray's bounds",
					                             "y");
				}

				m_values[ LinearIndex(x, y) ] = value;
			}
		}

		
		//======================================================================
		// Private Methods
		//======================================================================

		
		//----------------------------------------------------------------------
		uint WrapIndex(int index, uint size)
		{
			while (index < 0) 		index += (int)size;
			while (index >= size) 	index -= (int)size;
			return (uint)index;
		}

		
		//----------------------------------------------------------------------
		// Convert from offset 2D index to 1D index
		uint LinearIndex(uint x, uint y)
		{
			uint index =	WrapIndex((int)(y + m_originY), m_sizeY) * m_sizeX;
			index +=		WrapIndex((int)(x + m_originX), m_sizeX);
			return index;
		}


	} // Class definition
} // Namespace

