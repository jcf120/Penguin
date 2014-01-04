//------------------------------------------------------------------------------
// File:	Spline.cs
// Author:	Jocelyn Clifford-Frith
// Date:	4th January 2014
//
// Description:
// Takes a Vector3 arry at least 4 elements in size. Interpolating returns a
// point between points[1] and points[size-2], i.e. points[0] & points[size-1]
// are used only for anchoring purposes.
//------------------------------------------------------------------------------

using System;
using UnityEngine;
namespace Penguin
{
	//--------------------------------------------------------------------------
	public class Spline
	{
		
		
		//----------------------------------------------------------------------
		// Private members
		Vector3[] 		m_points;
		
		
		//======================================================================
		// Public Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------
		public Spline (Vector3[] points)
		{
			if (points.Length < 4)
			{
				throw new ArgumentException("Spline must be constructed with" +
				                            "at least 4 points.", "points");
			}
			
			m_points = points;
		}
		
		
		//----------------------------------------------------------------------
		public Vector3 Interpolate (float t)
		{
			uint 	i = 1;
			float 	u = 0.0f;

			FindSegmentStart(t, ref u, ref i);
			return Interpolate(m_points[i - 1],
			                   m_points[i    ],
			                   m_points[i + 1],
			                   m_points[i + 2],
			                   u);
		}
		
		
		//======================================================================
		// Private Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------
		// Sums the segment lengths between points[1] and points[size-2]
		float CalcLength()
		{
			float length = 0.0f;
			
			for (uint i = 1; i < m_points.Length - 3; i++)
			{
				Vector3 seg = m_points[i + 1] - m_points[i];
				length += seg.magnitude;
			}
			
			return length;
		}
		
		
		//----------------------------------------------------------------------
		// Finds the last point before the position represented by the t value
		void FindSegmentStart(float t, ref float startPos, ref uint startIndex)
		{
			float	posOnSpline			= t * CalcLength();
			float	cumulativeLength	= 0.0f;
			
			for (uint i = 1; i < m_points.Length - 3; i++)
			{
				startIndex 	= i;
				startPos	= cumulativeLength;

				Vector3 seg = m_points[i + 1] - m_points[i];
				cumulativeLength += seg.magnitude;

				if (cumulativeLength > posOnSpline)
					break;
			}
		}


		//----------------------------------------------------------------------
		Vector3 Interpolate(Vector3 p0,
		                    Vector3 p1,
		                    Vector3 p2,
		                    Vector3 p3,
		                    float u)
		{
			float u2 = u * u;
			float u3 = u2 * u;
			
			Vector3 t1 = 0.5f * (p2 - p0);
			Vector3 t2 = 0.5f * (p3 - p1);

			float b0 = 2 * u3 - 3 * u2 + 1;
			float b1 = -2 * u3 + 3 * u2;
			float b2 = u3 - 2 * u2 + u;
			float b3 = u3 - u2;

			return b0 * p0 + b1 * p1 + b2 * t1 + b3 * t2;
		}
		
		
	} // Class definition
} // Namespace

