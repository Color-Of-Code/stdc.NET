// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;

namespace QUT.Gplex.Parser
{
    internal class Error : IComparable<Error>
    {
        internal const int minErr = 50;
        internal const int minWrn = 110;

        internal int code;
        internal bool isWarn;
        internal string message;
        internal LexSpan span;


        internal Error(int code, string msg, LexSpan spn, bool wrn)
        {
            this.code = code;
            isWarn = wrn;
            message = msg;
            span = spn;
        }

        public int CompareTo(Error r)
        {
            if (span.startLine < r.span.startLine) return -1;
            else if (span.startLine > r.span.startLine) return 1;
            else if (span.startColumn < r.span.startColumn) return -1;
            else if (span.startColumn > r.span.startColumn) return 1;
            else return 0;
        }
    }
}
