//
// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)
// 
// These utilities are used in GPLEX and GPPG
//

using System;
using System.Runtime.Serialization;

namespace QUT.Gplib
{

    [Serializable]
    public class TooManyErrorsException : Exception
    {
        public TooManyErrorsException() { }
        public TooManyErrorsException(string message) : base(message) { }
        public TooManyErrorsException(string message, Exception inner) : base(message, inner) { }
        protected TooManyErrorsException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
