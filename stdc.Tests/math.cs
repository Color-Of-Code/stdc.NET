using System;

using NUnit.Framework;

namespace stdc.Tests
{

    [TestFixture]
    public class math
    {
        [Test(Description = "Domain errors")]
        public void DomainErrors()
        {
            Double ret = C.sqrt(-1.0);
            Assert.AreEqual(C.EDOM, C.errno);

            ret = C.log(0);
            Assert.AreEqual(C.ERANGE, C.errno);

            ret = C.log10(0);
            Assert.AreEqual(C.ERANGE, C.errno);
        }

        [Test(Description = "Range errors")]
        public void RangeErrors()
        {
            Double ret = C.log(0);
            Assert.AreEqual(C.ERANGE, C.errno);

            ret = C.log10(0);
            Assert.AreEqual(C.ERANGE, C.errno);
        }

    }
}
