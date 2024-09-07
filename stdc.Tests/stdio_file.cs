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
        Assert.That(f == NULL);
        Assert.That(errno == ENOENT);
    }

    [Test(Description = "opening file with illegal mode")]
    public void FOpenWithIllegalParameter()
    {
        FILE f = fopen("dum.my", "il");
        Assert.That(f == NULL);
        Assert.That(errno == EINVAL);
    }

    [Test(Description = "temporary file creation and cleanup")]
    public void TemporaryFile()
    {
        FILE f = tmpfile();
        Assert.That(f != NULL);
        String filename = f.Name;
        Assert.That(File.Exists(filename));
        fclose(f);
        Assert.That(File.Exists(filename), Is.False);
    }
}
