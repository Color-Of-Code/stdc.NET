// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;

namespace QUT.Gplib
{
    public class Error : IComparable<Error>
    {
        public int Code { get; private set; }
        public bool IsWarning  { get; private set; }
        public string Message { get; private set; }
        public ISpan Span { get; private set; }


        public Error(int code, string message, ISpan span, bool warning)
        {
            Code = code;
            IsWarning = warning;
            Message = message;
            Span = span;
        }
        
        public Error(string message, ISpan span, bool warning)
        {
            Code = -1;
            IsWarning = warning;
            Message = message;
            Span = span;
        }

        public int CompareTo(Error r)
        {
            if (Span.StartLine < r.Span.StartLine) return -1;
            
            if (Span.StartLine > r.Span.StartLine) return 1;
            
            if (Span.StartColumn < r.Span.StartColumn) return -1;
            
            if (Span.StartColumn > r.Span.StartColumn) return 1;
            
            return 0;
        }
    }
}
