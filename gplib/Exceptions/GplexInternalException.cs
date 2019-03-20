
using System;
using System.Runtime.Serialization;

namespace QUT.Gplib
{
    [Serializable]
    public class ToolInternalException : Exception
    {
        public ToolInternalException() { }
        public ToolInternalException(string message) : base(message) { }
        public ToolInternalException(string message, Exception inner) : base(message, inner) { }
        protected ToolInternalException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
