// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough QUT 2010
// (see accompanying GPPGcopyright.rtf)

using System;
using System.Runtime.Serialization;

namespace QUT.GPGen.Parser
{
    [Serializable]
    public class GppgInternalException : Exception {
        public GppgInternalException() { }
        public GppgInternalException(string message) : base(message) { }
        public GppgInternalException(string message, Exception innerException)
            : base(message, innerException) { }
        protected GppgInternalException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

}
