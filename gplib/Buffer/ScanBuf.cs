using System.Collections.Generic;
using System.IO;

namespace QUT.Gplib
{
    public abstract class ScanBuff : QUT.Gplib.IScanBuffer
    {
        private string fileNm;

        public const int EndOfFile = -1;
        public const int UnicodeReplacementChar = 0xFFFD;

        public bool IsFile { get { return (fileNm != null); } }
        public string FileName { get { return fileNm; } set { fileNm = value; } }

        public abstract int Pos { get; set; }
        public abstract int Read();
        public virtual void Mark() { }

        public abstract string GetString(int begin, int limit);

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
