//------------------------------------------------------------------------------
// File:	RandomisedStack.cs
// Author:	Jocelyn Clifford-Frith
// Date:	29th December 2013
//
// Description:
// A generic container that stores each element with an arbitrary value
// reflecting it's relative probablity of being popped.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
namespace Penguin
{
	public class RandomisedStack<T>
	{
		
		
		List<T> 	m_items;
		Random		m_random;
		
		
		public RandomisedStack ()
		{
			m_items	= new List<T>();
			m_random	= new Random();
		}
		
		
		public uint Size ()
		{
			return (uint)m_items.Count;
		}
		
		
		public bool IsEmpty ()
		{
			return Size() == 0;
		}
		
		
		public void Push (T item)
		{
			m_items.Add(item);
		}
		
		
		public T Peek ()
		{
			if ( IsEmpty() )
			{
				throw new InvalidOperationException("Cannot not peak into an empty stack");
			}
			
			return RandomItem();
		}
		
		
		public T Pop ()
		{
			if ( IsEmpty() )
			{
				throw new InvalidOperationException("Cannot pop from an empty stack");
			}
			
			T item = RandomItem();
			m_items.Remove(item);
			return item;
		}
		
		
		T RandomItem ()
		{
			int randIndex = (int)(m_random.NextDouble() * Size());
			
			return m_items[randIndex];
		}
	}
}

