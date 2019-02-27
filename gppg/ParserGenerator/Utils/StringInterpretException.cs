//
// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2010
// (see accompanying GPLEXcopyright.rtf)
// 
// These utilities are used in GPLEX and GPPG
//

using System;
using System.Runtime.Serialization;

namespace QUT.GPGen
{
    [Serializable]
    public class StringInterpretException : Exception
    {
        public StringInterpretException() { }
        public StringInterpretException(string message) : base(message) { }
        public StringInterpretException(string message, Exception innerException)
            : base(message, innerException) { }
        protected StringInterpretException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
