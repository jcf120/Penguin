using System;
using System.Diagnostics;

namespace Penguin
{
	public class DebugUtils
	{
		[Conditional("DEBUG")]
	    public static void Assert(bool condition)
	    {
	        if (!condition) throw new Exception();
	    }
		
		
		[Conditional("DEBUG")]
	    public static void Assert(bool condition, string message)
	    {
	        if (!condition) throw new Exception(message);
	    }
	}
}

