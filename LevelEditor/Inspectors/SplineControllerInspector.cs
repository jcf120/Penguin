//------------------------------------------------------------------------------
// File:	SplineControllerInspector.cs
// Author:	Jocelyn Clifford-Frith
// Date:	25th January 2014
//
// Description:
// 
//------------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;
using Penguin;
namespace LevelEditor
{
	//--------------------------------------------------------------------------
	[ CustomEditor( typeof(SplineController) ) ] 
	public class SplineControllerInspector : Editor
	{
		
		
		//----------------------------------------------------------------------
		// Private members
		
		
		//======================================================================
		// Public Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------
		public void OnSceneGUI ()
		{
			SplineController splineTarget = (SplineController)target;

			Vector3[] points = splineTarget.GetPoints ();

			for (int i = 0; i < points.Length; i++)
			{
				Handles.DotCap (0, points[i], Quaternion.identity, 0.1f);
			}

			Spline spline = splineTarget.GetSpline ();

			int segs = 10;
			Vector3[] splineVerts = new Vector3[segs + 1];
			for (int i = 0; i <= segs; i++)
			{
				float t = i / (float)segs;
				splineVerts[i] = spline.InterpolatePoint (t);
			}

			Handles.DrawPolyLine (splineVerts);
		}
		
		
		
		//======================================================================
		// Private Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------
		
		
		
	} // Class definition
} // Namespace

