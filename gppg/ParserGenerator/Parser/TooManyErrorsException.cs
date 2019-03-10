// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough QUT 2010
// (see accompanying GPPGcopyright.rtf)

using System;
using System.Runtime.Serialization;

namespace QUT.GPGen.Parser
{
    [Serializable]
    public class TooManyErrorsException : Exception {
        public TooManyErrorsException() { }
        public TooManyErrorsException(string message) : base(message) { }
        public TooManyErrorsException(string message, Exception innerException)
            : base(message, innerException) { }
        protected TooManyErrorsException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

}
