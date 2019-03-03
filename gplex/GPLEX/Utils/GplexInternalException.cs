//
// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)
// 
// These utilities are used in GPLEX and GPPG
//

using System;
using System.Runtime.Serialization;

namespace QUT.Gplex.Parser
{
    [Serializable]
    public class GplexInternalException : Exception
    {
        public GplexInternalException() { }
        public GplexInternalException(string message) : base(message) { }
        public GplexInternalException(string message, Exception inner) : base(message, inner) { }
        protected GplexInternalException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
