using System.Collections.Generic;
using System.Text;

namespace QUT.Gplib
{
    // ==============================================================
    //  The LineBuff class contributed by Nigel Horspool, 
    //  nigelh@cs.uvic.cs
    // ==============================================================

    sealed class LineBuffer : ScanBuff
    {
        IList<string> line;    // list of source lines from a file
        int numLines;          // number of strings in line list
        string curLine;        // current line in that list
        int cLine;             // index of current line in the list
        int curLen;            // length of current line
        int curLineStart;      // position of line start in whole file
        int curLineEnd;        // position of line end in whole file
        int maxPos;            // max position ever visited in whole file
        int cPos;              // ordinal number of code in source

        // Constructed from a list of strings, one per source line.
        // The lines have had trailing '\n' characters removed.
        public LineBuffer(IList<string> lineList)
        {
            line = lineList;
            numLines = line.Count;
            cPos = curLineStart = 0;
            curLine = (numLines > 0 ? line[0] : "");
            maxPos = curLineEnd = curLen = curLine.Length;
            cLine = 1;
            FileName = null;
        }

        public override int Read()
        {
            if (cPos < curLineEnd)
                return curLine[cPos++ - curLineStart];
            if (cPos++ == curLineEnd)
                return '\n';
            if (cLine >= numLines)
                return EndOfFile;
            curLine = line[cLine];
            curLen = curLine.Length;
            curLineStart = curLineEnd + 1;
            curLineEnd = curLineStart + curLen;
            if (curLineEnd > maxPos)
                maxPos = curLineEnd;
            cLine++;
            return curLen > 0 ? curLine[0] : '\n';
        }

        // To speed up searches for the line containing a position
        private int cachedPosition;
        private int cachedIxdex;
        private int cachedLineStart;

        // Given a position pos within the entire source, the results are
        //   ix     -- the index of the containing line
        //   lstart -- the position of the first character on that line
        private void findIndex(int pos, out int ix, out int lstart)
        {
            if (pos >= cachedPosition)
            {
                ix = cachedIxdex; lstart = cachedLineStart;
            }
            else
            {
                ix = lstart = 0;
            }
            while (ix < numLines)
            {
                int len = line[ix].Length + 1;
                if (pos < lstart + len) break;
                lstart += len;
                ix++;
            }
            cachedPosition = pos;
            cachedIxdex = ix;
            cachedLineStart = lstart;
        }

        public override string GetString(int begin, int limit)
        {
            if (begin >= maxPos || limit <= begin) return "";
            int endIx, begIx, endLineStart, begLineStart;
            findIndex(begin, out begIx, out begLineStart);
            int begCol = begin - begLineStart;
            findIndex(limit, out endIx, out endLineStart);
            int endCol = limit - endLineStart;
            string s = line[begIx];
            if (begIx == endIx)
            {
                // the usual case, substring all on one line
                return (endCol <= s.Length) ?
                    s.Substring(begCol, endCol - begCol)
                    : s.Substring(begCol) + "\n";
            }
            // the string spans multiple lines, yuk!
            StringBuilder sb = new StringBuilder();
            if (begCol < s.Length)
                sb.Append(s.Substring(begCol));
            for (; ; )
            {
                sb.Append("\n");
                s = line[++begIx];
                if (begIx >= endIx) break;
                sb.Append(s);
            }
            if (endCol <= s.Length)
            {
                sb.Append(s.Substring(0, endCol));
            }
            else
            {
                sb.Append(s);
                sb.Append("\n");
            }
            return sb.ToString();
        }

        public override int Pos
        {
            get { return cPos; }
            set
            {
                cPos = value;
                findIndex(cPos, out cLine, out curLineStart);
                // cLine should be the *next* line after curLine.
                curLine = (cLine < numLines ? line[cLine++] : "");
                curLineEnd = curLineStart + curLine.Length;
            }
        }

        public override string ToString() { return "LineBuffer"; }
    }
}
