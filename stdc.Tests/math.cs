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
        Assert.That(errno, Is.EqualTo(EDOM));

        ret = log(0);
        Assert.That(errno, Is.EqualTo(ERANGE));

        ret = log10(0);
        Assert.That(errno, Is.EqualTo(ERANGE));
    }

    [Test(Description = "Range errors")]
    public void RangeErrors()
    {
        Double ret = log(0);
        Assert.That(errno, Is.EqualTo(ERANGE));

        ret = log10(0);
        Assert.That(errno, Is.EqualTo(ERANGE));
    }

}
