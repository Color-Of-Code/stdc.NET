using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace stdc.Tests {

	[TestFixture]
	public class stdio_file {

		[Test (Description = "opening unexisting file")]
		public void FOpenWithUnexistingFile ()
		{
			FILE f = C.fopen ("dum.my", "rb");
			Assert.IsTrue (f == C.NULL);
			Assert.IsTrue (C.errno == C.ENOENT);
		}

		[Test (Description = "opening file with illegal mode")]
		public void FOpenWithIllegalParameter ()
		{
			FILE f = C.fopen ("dum.my", "il");
			Assert.IsTrue (f == C.NULL);
			Assert.IsTrue (C.errno == C.EINVAL);
		}
	}
}
