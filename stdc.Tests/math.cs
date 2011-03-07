using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace stdc.Tests {

	[TestFixture]
	public class math {
		[Test (Description = "Domain errors")]
		public void DomainErrors ()
		{
			Double ret = C.sqrt (-1.0);
			Assert.AreEqual (C.EDOM, C.errno);
			
			ret = C.log (0);
			Assert.AreEqual (C.ERANGE, C.errno);

			ret = C.log (-1);
			Assert.AreEqual (C.EDOM, C.errno);

			ret = C.log10 (0);
			Assert.AreEqual (C.ERANGE, C.errno);

			ret = C.log10 (-1);
			Assert.AreEqual (C.EDOM, C.errno);
		}

	}
}
