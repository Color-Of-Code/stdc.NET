using System;
using System.IO;

using NUnit.Framework;

namespace stdc.Tests
{

    [TestFixture]
    public class stdio_file
    {

        [Test(Description = "opening unexisting file")]
        public void FOpenWithUnexistingFile()
        {
            C.FILE f = C.fopen("dum.my", "rb");
            Assert.IsTrue(f == C.NULL);
            Assert.IsTrue(C.errno == C.ENOENT);
        }

        [Test(Description = "opening file with illegal mode")]
        public void FOpenWithIllegalParameter()
        {
            C.FILE f = C.fopen("dum.my", "il");
            Assert.IsTrue(f == C.NULL);
            Assert.IsTrue(C.errno == C.EINVAL);
        }

        [Test(Description = "temporary file creation and cleanup")]
        public void TemporaryFile()
        {
            C.FILE f = C.tmpfile();
            Assert.IsTrue(f != C.NULL);
            String filename = f.Name;
            Assert.IsTrue(File.Exists(filename));
            C.fclose(f);
            Assert.IsFalse(File.Exists(filename));
        }
    }
}
