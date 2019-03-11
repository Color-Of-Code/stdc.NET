// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;

namespace QUT.Gplib
{
    public class Error : IComparable<Error>
    {
        public const int minWrn = 110;

        public int Code { get; private set; }
        public bool IsWarning  { get; private set; }
        public string Message { get; private set; }
        public ISpan Span { get; private set; }


        public Error(int code, string msg, ISpan spn, bool wrn)
        {
            this.Code = code;
            IsWarning = wrn;
            Message = msg;
            Span = spn;
        }
        
        public Error(string msg, ISpan spn, bool wrn)
        {
            this.Code = -1;
            IsWarning = wrn;
            Message = msg;
            Span = spn;
        }

        public int CompareTo(Error r)
        {
            if (Span.startLine < r.Span.startLine) return -1;
            
            if (Span.startLine > r.Span.startLine) return 1;
            
            if (Span.startColumn < r.Span.startColumn) return -1;
            
            if (Span.startColumn > r.Span.startColumn) return 1;
            
            return 0;
        }
    }
}
