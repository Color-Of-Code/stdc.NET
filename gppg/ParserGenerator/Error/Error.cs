// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, K John Gough, QUT 2006-2010
// (see accompanying GPPGcopyright.rtf)
// This file author: John Gough, borrowed from GPLEX

using System;

namespace QUT.GPGen.Parser
{
    internal class Error : IComparable<Error>
    {
        internal const int minErr = 50;
        internal const int minWrn = 100;

        internal bool isWarn;
        internal string message;
        internal LexSpan span;


        internal Error(string msg, LexSpan spn, bool wrn)
        {
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