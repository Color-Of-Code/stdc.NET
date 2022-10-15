namespace stdc.Tests;

using System;
using NUnit.Framework;

[TestFixture]
public class math : C
{
    [Test(Description = "Domain errors")]
    public void DomainErrors()
    {
        Double ret = sqrt(-1.0);
        Assert.AreEqual(EDOM, errno);

        ret = log(0);
        Assert.AreEqual(ERANGE, errno);

        ret = log10(0);
        Assert.AreEqual(ERANGE, errno);
    }

    [Test(Description = "Range errors")]
    public void RangeErrors()
    {
        Double ret = log(0);
        Assert.AreEqual(ERANGE, errno);

        ret = log10(0);
        Assert.AreEqual(ERANGE, errno);
    }

}
