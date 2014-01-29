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
		public Vector3 InterpolatePoint (float t)
		{
			uint 	i = 1;
			float 	u = 0.0f;
			
			FindSegmentStart(t, ref u, ref i);
			return InterpolatePoint(m_points[i - 1],
			                        m_points[i    ],
			                        m_points[i + 1],
			                        m_points[i + 2],
			                        u);
		}
		
		
		//----------------------------------------------------------------------
		// Same again, but returning info about position in spline relative to
		// the splines defining points
		public Vector3 InterpolatePoint (float t, ref uint i, ref float u)
		{
			FindSegmentStart(t, ref u, ref i);
			return InterpolatePoint(m_points[i - 1],
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
		Vector3 InterpolatePoint(Vector3 a,
		                         Vector3 b,
		                         Vector3 c,
		                         Vector3 d,
		                         float u)
		{
			Vector3 p1 = (d - 3 * c) + (3 * b - a);
			Vector3 p2 = 3 * (c - (2 * b) + a);
			Vector3 p3 = 3 * (b - a);
			
			float u2 = u * u;
			float u3 = u2 * u;
			
			return (p1 * u3) + (p2 * u2) + (p3 * u) + a;
		}
		
		
		//----------------------------------------------------------------------
		Vector3 InterpolateTangent(Vector3 a,
		                           Vector3 b,
		                           Vector3 c,
		                           Vector3 d,
		                           float u)
		{
			Vector3 p1 = (d - 3 * c) + (3 * b - a);
			Vector3 p2 = 3 * (c - (2 * b) + a);
			Vector3 p3 = 3 * (b - a);
			
			float u2 = u * u;
			
			return (3 * p1 * u2) + (2 * p2 * u) + p3;
		}
		
		
	} // Class definition
} // Namespace

