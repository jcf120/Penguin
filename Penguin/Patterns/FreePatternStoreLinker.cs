using System;
using System.Collections.Generic;
using UnityEngine;
using Penguin;

namespace Penguin
{
	public class FreePatternStoreLinker
	{
		public static void linkPacToStores(PatternArrayController pac, Dictionary<string, FreePatternStore> storesDict)
		{
			foreach (CellPattern pat in pac.patterns) {
				
				// Is it a FreePattern
				if (pat.GetType() == typeof(FreePattern)) {
					FreePattern freePat = pat as FreePattern;
					
					// Warn if already linked
					if (freePat.store != null)
						Debug.LogWarning("FreePattern's store '"+freePat.storeName+"' already linked - overwriting");
					
					// Warn if FreePattern has specified a store
					if (freePat.storeName == "unassigned") {
						Debug.LogWarning("FreePattern's store has not been specified");
						break;
					}
					
					// Does dict contain store?
					if (storesDict.ContainsKey(freePat.storeName)) {
						freePat.store = storesDict[freePat.storeName];
					}
					else {
						Debug.LogError("FreePatternStoreLinker not passed store '"+freePat.storeName+"'.");
					}
				}
			}
		}
	}
}

