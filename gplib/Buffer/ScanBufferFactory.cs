using System.Collections.Generic;
using System.IO;

namespace QUT.Gplib
{
    public static class ScanBufferFactory
    {
        public static IScanBuffer GetBuffer(string source)
        {
            return new StringBuffer(source);
        }

        public static IScanBuffer GetBuffer(IList<string> source)
        {
            return new LineBuffer(source);
        }

        // #if (!NOFILES)
        public static IScanBuffer GetBuffer(Stream source)
        {
            return new BuildBuffer(source);
        }

        // #if (!NOFILES)
        // #if (!BYTEMODE)
        public static IScanBuffer GetBuffer(Stream source, int fallbackCodePage)
        {
            return new BuildBuffer(source, fallbackCodePage);
        }
    }
}
