namespace stdc.Tests;

using System;
using System.IO;
using NUnit.Framework;

[TestFixture]
public class stdio_file : C
{

    [Test(Description = "opening unexisting file")]
    public void FOpenWithUnexistingFile()
    {
        FILE f = fopen("dum.my", "rb");
        Assert.IsTrue(f == NULL);
        Assert.IsTrue(errno == ENOENT);
    }

    [Test(Description = "opening file with illegal mode")]
    public void FOpenWithIllegalParameter()
    {
        FILE f = fopen("dum.my", "il");
        Assert.IsTrue(f == NULL);
        Assert.IsTrue(errno == EINVAL);
    }

    [Test(Description = "temporary file creation and cleanup")]
    public void TemporaryFile()
    {
        FILE f = tmpfile();
        Assert.IsTrue(f != NULL);
        String filename = f.Name;
        Assert.IsTrue(File.Exists(filename));
        fclose(f);
        Assert.IsFalse(File.Exists(filename));
    }
}
