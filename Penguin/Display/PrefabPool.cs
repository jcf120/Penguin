//------------------------------------------------------------------------------
// File:	PrefabPool.cs
// Author:	Jocelyn Clifford-Frith
// Date:	29th December 2013
//
// Description:
// PrefabPool holds on to prefab instances while they are inactive so as to cut
// down on unecessary allocations. A single PrefabPool is intended to manage
// prefabs with the same logical representation but potentially different
// appearances. This is done with a SpawningDesc(riptor), which states the
// likelihood of each prefab variety being instantiated.
//------------------------------------------------------------------------------

using System;
using UnityEngine;
namespace Penguin
{
	//--------------------------------------------------------------------------
	// Typedefs
	using SpawningDesc = WeightedRandomisedStack<GameObject>;


	//--------------------------------------------------------------------------
	public class PrefabPool
	{


		//----------------------------------------------------------------------
		// Private members
		SpawningDesc					m_spawningDesc;
		RandomisedStack<GameObject>		m_instances;


		//----------------------------------------------------------------------
		public PrefabPool (SpawningDesc spawningDesc)
		{
			m_instances = new RandomisedStack<GameObject>();

			m_spawningDesc = spawningDesc;
		}
		
		
		//----------------------------------------------------------------------
		public GameObject RetrieveInstance ()
		{
			GameObject instance = null;
			
			if ( m_instances.IsEmpty () )
			{
				// choose which prefab to instantiate
				GameObject prefab = m_spawningDesc.Peek ();
				instance = InstanciatePrefab (prefab);
			}
			else
			{
				// otherwise pop an existing one
				instance = m_instances.Pop ();
			}

			return instance;
		}

		
		//----------------------------------------------------------------------
		public void StoreInstance (GameObject instance)
		{
			if ( !HasPrefabForInstance (instance) )
			{
				Debug.LogError("PrefabPool doesn't support instance with name" +
				               instance.name + "',");
			}

			if ( instance.activeInHierarchy )
			{
				Debug.LogWarning ("Stored prefab instance '" +
				                 instance.name +
				                 "' hadn't been deactivated.");
				instance.SetActive (false);
			}

			m_instances.Push (instance);
		}

		
		//----------------------------------------------------------------------
		GameObject InstanciatePrefab (GameObject prefab)
		{
			return GameObject.Instantiate(prefab) as GameObject;
		}

		
		//----------------------------------------------------------------------
		bool HasPrefabForInstance (GameObject instance)
		{
			bool result = false;

			GameObject[] prefabs = m_spawningDesc.ToArray ();

			foreach (GameObject p in prefabs)
			{
				if (p.name == instance.name)
				{
					result = true;
					break;
				}
			}

			return result;
		}


	} // Class Definition
} // Namespace

