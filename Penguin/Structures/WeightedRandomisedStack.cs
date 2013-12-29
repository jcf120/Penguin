//------------------------------------------------------------------------------
// File:	WeightedRandomisedStack.cs
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
	public class WeightedRandomisedStack<T>
	{

		struct Choice
		{
			public T		item;
			public double	weighting;
		}


		List<Choice> 	m_choices;
		Random			m_random;


		public WeightedRandomisedStack ()
		{
			m_choices	= new List<Choice>();
			m_random	= new Random();
		}


		public uint Size ()
		{
			return (uint)m_choices.Count;
		}


		public bool IsEmpty ()
		{
			return Size() == 0;
		}


		public void Push (T item, double weighting)
		{
			Choice choice;
			choice.item			= item;
			choice.weighting	= weighting;
			m_choices.Add(choice);
		}


		public T Peek ()
		{
			if ( IsEmpty() )
			{
				throw new InvalidOperationException("Cannot not peak into an empty stack");
			}

			return RandomChoice().item;
		}


		public T Pop ()
		{
			if ( IsEmpty() )
			{
				throw new InvalidOperationException("Cannot pop from an empty stack");
			}

			Choice choice = RandomChoice();
			m_choices.Remove(choice);
			return choice.item;
		}


		Choice RandomChoice ()
		{
			Choice returnChoice = new Choice();
			
			double randSample = m_random.NextDouble() * CalcProbabilitySpaceSize();
			double cumulativeProbability = 0.0;
			
			foreach (Choice c in m_choices)
			{
				cumulativeProbability += c.weighting;
				
				if (randSample < cumulativeProbability)
				{
					returnChoice = c;
					break;
				}
			}
			
			return returnChoice;
		}


		// Sums the the weightings of all choices
		double CalcProbabilitySpaceSize ()
		{
			double total = 0.0;

			foreach (Choice c in m_choices)
			{
				total += c.weighting;
			}

			return total;
		}
	}
}

