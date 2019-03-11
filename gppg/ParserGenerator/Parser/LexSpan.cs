// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough QUT 2010
// (see accompanying GPPGcopyright.rtf)

using QUT.GplexBuffers;
using QUT.Gplib;

namespace QUT.GPGen.Parser
{
    // ===================================================================
    // ===================================================================

    /// <summary>
    /// Objects of this class represent locations in the input text.
    /// The fields record both line:column information and also 
    /// file position data and buffer object identity.
    /// </summary>
    internal class LexSpan : IMerge<LexSpan>, ISpan
    {
        public int startLine { get; private set; }       // start line of span
        public int startColumn { get; private set; }     // start column of span
        public int endLine { get; private set; }         // end line of span
        public int endColumn { get; private set; }       // end column of span
        internal int startIndex;    // start position in the buffer
        internal int endIndex;      // end position in the buffer
        internal ScanBuff buffer;   // reference to the buffer

        public LexSpan() { }
        public LexSpan(int sl, int sc, int el, int ec, int sp, int ep, ScanBuff bf)
        { startLine = sl; startColumn = sc; endLine = el; endColumn = ec; startIndex = sp; endIndex = ep; buffer = bf; }

        /// <summary>
        /// This method implements the IMerge interface
        /// </summary>
        /// <param name="end">The last span to be merged</param>
        /// <returns>A span from the start of 'this' to the end of 'end'</returns>
        public LexSpan Merge(LexSpan end)
        {
            return new LexSpan(startLine, startColumn, end.endLine, end.endColumn, startIndex, end.endIndex, buffer);
        }

        //internal bool IsInitialized { get { return buffer != null; } }

        /// <summary>
        /// Write the text of this text span to the stream
        /// </summary>
        /// <param name="sWtr"></param>
        //internal void StreamDump(TextWriter sWtr)
        //{
        //    int savePos = buffer.Pos;
        //    string str = buffer.GetString(startIndex, endIndex);

        //    sWtr.WriteLine(str);
        //    buffer.Pos = savePos;
        //    sWtr.Flush();
        //}

        /// <summary>
        /// Write the text of this text span to the console
        /// </summary>
        //internal void ConsoleDump()
        //{
        //    int savePos = buffer.Pos;
        //    string str = buffer.GetString(startIndex, endIndex);
        //    Console.WriteLine(str);
        //    buffer.Pos = savePos;
        //}

        public override string ToString()
        {
            return buffer.GetString(startIndex, endIndex);
        }
    }

}
