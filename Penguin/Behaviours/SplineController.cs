//------------------------------------------------------------------------------
// File:	SplineController.cs
// Author:	Jocelyn Clifford-Frith
// Date:	24th January 2014
//
// Description:
// 
//------------------------------------------------------------------------------

using System;
using UnityEngine;
namespace Penguin
{
	//--------------------------------------------------------------------------
	public class SplineController : MonoBehaviour
	{
		
		
		//----------------------------------------------------------------------
		// Private members
		
		
		//======================================================================
		// Public Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------
		public void Start ()
		{

		}
		
		
		//----------------------------------------------------------------------
		public Vector3[] GetPoints ()
		{
			Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
			
			Vector3[] points = new Vector3[transforms.Length];
			for (int i = 0; i < transforms.Length; i++)
			{
				points[i] = transforms[i].position;
			}
			
			return points;
		}
		
		
		//----------------------------------------------------------------------
		public Spline GetSpline ()
		{
			return new Spline ( GetPoints() );
		}
		
		
		
		//======================================================================
		// Private Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------

		
		
	} // Class definition
} // Namespace

