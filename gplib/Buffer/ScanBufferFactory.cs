using System.Collections.Generic;
using System.IO;

namespace QUT.Gplib
{
    public static class ScanBufferFactory
    {
        public static ScanBuff GetBuffer(string source)
        {
            return new StringBuffer(source);
        }

        public static ScanBuff GetBuffer(IList<string> source)
        {
            return new LineBuffer(source);
        }

        // #if (!NOFILES)
        public static ScanBuff GetBuffer(Stream source)
        {
            return new BuildBuffer(source);
        }

        // #if (!NOFILES)
        // #if (!BYTEMODE)
        public static ScanBuff GetBuffer(Stream source, int fallbackCodePage)
        {
            return new BuildBuffer(source, fallbackCodePage);
        }
    }
}
