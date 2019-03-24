using System.Collections.Generic;
using System.IO;

namespace QUT.Gplib
{
    public static class ScanBuffCode
    {
        public const int EndOfFile = -1;
        public const int UnicodeReplacementChar = 0xFFFD;
    }

    public abstract class ScanBuff : IScanBuffer
    {
        public string FileName { get; set; }

        public abstract int Pos { get; set; }
        public abstract int Read();
        public virtual void Mark() { }

        public abstract string GetString(int begin, int limit);
    }
}
