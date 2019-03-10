// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf.

//  Parse helper for bootstrap version of gplex
//  kjg 17 June 2006
//

using System.IO;
using QUT.GplexBuffers;

namespace QUT.Gplex.Parser
{
    internal class LexSpan : QUT.Gplib.IMerge<LexSpan>
    {
        internal int startLine;       // start line of span
        internal int startColumn;     // start column of span
        internal int endLine;         // end line of span
        internal int endColumn;       // end column of span
        internal int startIndex;      // start position in the buffer
        internal int endIndex;        // end position in the buffer
        internal ScanBuff buffer;     // reference to the buffer

        public LexSpan() { }
        public LexSpan(int sl, int sc, int el, int ec, int sp, int ep, ScanBuff bf)
        { 
            startLine = sl;
            startColumn = sc;
            endLine = el;
            endColumn = ec;
            startIndex = sp;
            endIndex = ep;
            buffer = bf;
        }

        /// <summary>
        /// This method implements the IMerge interface
        /// </summary>
        /// <param name="end">The last span to be merged</param>
        /// <returns>A span from the start of 'this' to the end of 'end'</returns>
        public LexSpan Merge(LexSpan end)
        {
            return new LexSpan(startLine, startColumn, end.endLine, end.endColumn, startIndex, end.endIndex, buffer);
        }

        /// <summary>
        /// Get a short span from the first line of this span.
        /// </summary>
        /// <param name="idx">Starting index</param>
        /// <param name="len">Length of span</param>
        /// <returns></returns>
        internal LexSpan FirstLineSubSpan(int idx, int len)
        {
            return new LexSpan(
                this.startLine, this.startColumn + idx, this.startLine, this.startColumn + idx + len,
                this.startIndex, this.endIndex, this.buffer);
        }

        internal bool IsInitialized { get { return buffer != null; } }

        internal void StreamDump(TextWriter sWtr)
        {
            int savePos = buffer.Pos;
            string str = buffer.GetString(startIndex, endIndex);
            sWtr.WriteLine(str);
            buffer.Pos = savePos;
            sWtr.Flush();
        }

        public override string ToString()
        {
            return buffer.GetString(startIndex, endIndex);
        }
    }
}

