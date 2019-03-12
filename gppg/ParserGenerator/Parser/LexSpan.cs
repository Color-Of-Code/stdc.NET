// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough QUT 2010
// (see accompanying GPPGcopyright.rtf)

using QUT.GplexBuffers;
using QUT.Gplib;

namespace QUT.GPGen.Parser
{
    /// <summary>
    /// Objects of this class represent locations in the input text.
    /// The fields record both line:column information and also 
    /// file position data and buffer object identity.
    /// </summary>
    internal class LexSpan : IMerge<LexSpan>, ISpan
    {
        public int StartLine { get; private set; }       // start line of span
        public int StartColumn { get; private set; }     // start column of span
        public int EndLine { get; private set; }         // end line of span
        public int EndColumn { get; private set; }       // end column of span
        internal int startIndex;    // start position in the buffer
        internal int endIndex;      // end position in the buffer
        internal ScanBuff buffer;   // reference to the buffer

        public LexSpan() { }
        public LexSpan(int sl, int sc, int el, int ec, int sp, int ep, ScanBuff bf)
        { StartLine = sl; StartColumn = sc; EndLine = el; EndColumn = ec; startIndex = sp; endIndex = ep; buffer = bf; }

        /// <summary>
        /// This method implements the IMerge interface
        /// </summary>
        /// <param name="end">The last span to be merged</param>
        /// <returns>A span from the start of 'this' to the end of 'end'</returns>
        public LexSpan Merge(LexSpan end)
        {
            return new LexSpan(StartLine, StartColumn, end.EndLine, end.EndColumn, startIndex, end.endIndex, buffer);
        }

        public override string ToString()
        {
            return buffer.GetString(startIndex, endIndex);
        }
    }

}
