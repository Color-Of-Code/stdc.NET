using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace stdc {
	public static partial class c {

		public static void assert(Boolean condition)
		{
			Debug.Assert(condition);
		}

		public static void assert(Boolean condition, String message)
		{
			Debug.Assert(condition, message);
		}
	}
}
