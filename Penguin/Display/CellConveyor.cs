//------------------------------------------------------------------------------
// File:	CellConveyor.cs
// Author:	Jocelyn Clifford-Frith
// Date:	5th January 2014
//
// Description:
// 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
namespace Penguin
{
	//--------------------------------------------------------------------------
	public class CellConveyor
	{
		
		
		//----------------------------------------------------------------------
		struct CellInfo
		{
			public GameObject	gameObject;
			public float		depth;
		}


		//----------------------------------------------------------------------
		// Private members
		TorusArray<CellInfo>				m_cellTorus;
		Dictionary<CellType, PrefabPool>	m_poolDict;
		OrthoSplineArray					m_surface;
		Vector2								m_position;
		
		//======================================================================
		// Public Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------
		public CellConveyor (uint cols, uint rows)
		{
			m_cellTorus = new TorusArray<CellInfo> (cols, rows);
			PopulateTorus ();
		}
		
		
		//======================================================================
		// Private Methods
		//======================================================================
		
		
		//----------------------------------------------------------------------
		void PopulateTorus ()
		{
			for (uint i = 0; i < m_cellTorus.SizeX (); i++)
			{
				for (uint j = 0; j < m_cellTorus.SizeY (); j++)
				{
					CellInfo c = m_cellTorus[i, j];
					c.gameObject = GameObject.CreatePrimitive (PrimitiveType.Cube);
					c.depth = 0.0f;
				}
			}
		}


		//----------------------------------------------------------------------
		void UpdateCellPositions ()
		{
			uint cols = m_cellTorus.SizeX ();
			uint rows = m_cellTorus.SizeY ();

			for (uint i = 0; i < cols; i++)
			{
				for (uint j = 0; j < rows; j++)
				{
					Vector3 pos  = new Vector3();
					Vector3 norm = new Vector3();
					m_surface.Interpolate (i, j * 2, ref pos, ref norm);

					CellInfo c = m_cellTorus[i, j];
					c.gameObject.transform.position = pos;
				}
			}
		}
		
		
	} // Class definition
} // Namespace

