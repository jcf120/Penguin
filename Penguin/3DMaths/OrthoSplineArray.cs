//------------------------------------------------------------------------------
// File:	OrthoSplineArray.cs
// Author:	Jocelyn Clifford-Frith
// Date:	4th January 2014
//
// Description:
// 
//------------------------------------------------------------------------------

using System;
using UnityEngine;
namespace Penguin
{
	//--------------------------------------------------------------------------
	public class OrthoSplineArray
	{
		
		
		//----------------------------------------------------------------------
		// Private members
		Spline[] 		m_sourceSplines;
		Spline[]		m_orthoSplines;
		uint			m_segmentsY; // usually double the cell rows
		float			m_offsetX;
		float			m_offsetY;
		
		
		//======================================================================
		// Public Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------
		public OrthoSplineArray (Spline[] splines,
		                   		 uint segmentsX,
		                   		 uint segmentsY)
		{
			if (splines.Length < 4)
			{
				throw new ArgumentException("SurfaceNurb must be constructed " +
				                            "with at least 4 splines.",
				                            "splines");
			}
			
			m_sourceSplines = splines;
			m_segmentsY 	= segmentsY;
			m_offsetX		= 0f;
			m_offsetY		= 0f;

			m_orthoSplines 	= new Spline[segmentsX];
		}
		
		
		//----------------------------------------------------------------------
		public void Interpolate (uint x,
		                         uint y,
		                         ref Vector3 position,
		                         ref Vector3 normal)
		{
			if ( x < 0 || x >= m_orthoSplines.Length )
				throw new IndexOutOfRangeException("x out of range");
			
			if ( y < 0 || y >= m_segmentsY )
				throw new IndexOutOfRangeException("y out of range");

			float ty = (y + m_offsetY) / m_segmentsY;
			position = m_orthoSplines[x].InterpolatePoint (ty);
		}
		
		
		//----------------------------------------------------------------------
		public void SetOffset (uint x, uint y)
		{

		}
		
		
		//======================================================================
		// Private Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------
		void BuildOrthoSplines ()
		{
			uint segmentsX = (uint)m_orthoSplines.Length;
			for (int i = 0; i < segmentsX; i++)
			{
				float tx = (i + m_offsetX) / segmentsX;
				m_orthoSplines[i] = CalcOrthoSpline (tx);
			}
		}
		
		
		//----------------------------------------------------------------------
		Spline CalcOrthoSpline (float t)
		{
			Vector3[] points = new Vector3[m_sourceSplines.Length];
			
			for (int i = 0; i < m_sourceSplines.Length; i++)
			{
				points[i] = m_sourceSplines[i].InterpolatePoint (t);
			}
			
			return new Spline (points);
		}
		
		
	} // Class definition
} // Namespace

