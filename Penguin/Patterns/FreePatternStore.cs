using System;
using System.Collections.Generic;
using UnityEngine;
using Penguin;

namespace Penguin
{
	public class FreePatternStore : ScriptableObject
	{
		public int width;
		public int height;
		public CellType[] values;
		public CellType this[int col, int row]
		{
			get {return values[width*row + col];}
			set {values[width*row + col] = value;}
		}
		
		
		public Dictionary<string, object> packDict()
		{
			List<int> packedValues = new List<int>();
			
			// Rather than convert each enum in the array to a string, first index only used types into ints
			Dictionary<CellType,int> typeDict = new Dictionary<CellType, int>();
			
			for (int i=0; i<width*height; i++) {
				
				CellType ct = values[i];
					
				// Has type been indexed?
				if (!typeDict.ContainsKey(ct)) {
					// Then add it to the index
					typeDict[ct] = typeDict.Count;
				}
				
				packedValues.Add(typeDict[ct]);
			}
			
			Dictionary<string, object> data = new Dictionary<string, object>();
			data["class"   ] = GetType().ToString();
			data["typeDict"] = typeDict;
			data["height"  ] = height;
			data["width"   ] = width;
			data["values"  ] = packedValues;
			return data;
		}
		
		
		public void unpackDict(Dictionary<string, object> data)
		{
			// Check data represents this class
			if (data["class"].ToString() != GetType().ToString()) {
				Debug.LogError("Data used to unpack FreePatternStore doesn't represent class. Unpacking aborted");
				return;
			}
			
			Dictionary<int, CellType> reverseTypeDict = new Dictionary<int, CellType>();
			Dictionary<string, object> typeDict = data["typeDict"] as Dictionary<string, object>;
			foreach (KeyValuePair<string, object> kvp in typeDict) {
				reverseTypeDict[Convert.ToInt32(kvp.Value)] = (CellType)Enum.Parse(typeof(CellType), kvp.Key.ToString());
			}
			
			height = Convert.ToInt32(data["height"]);
			width  = Convert.ToInt32(data["width" ]);
			values = new CellType[width*height];
			
			List<object> packedValues = data["values"] as List<object>;
			for (int i=0; i<width*height; i++) {
				CellType ct = reverseTypeDict[Convert.ToInt32(packedValues[i])];
				values[i] = ct;
			}
		}
	}
}

