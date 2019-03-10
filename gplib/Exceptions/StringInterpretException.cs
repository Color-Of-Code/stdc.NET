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
    public class StringInterpretException : Exception
    {
        [NonSerialized]
        readonly string key;
        public string Key { get { return key; } }

        public StringInterpretException() { }
        public StringInterpretException(string text) : base(text) { }
        public StringInterpretException(string text, string key) : base(text) { this.key = key; }
        public StringInterpretException(string message, Exception inner) : base(message, inner) { }
        protected StringInterpretException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

    }
}
