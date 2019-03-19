using System;
using System.Runtime.Serialization;

namespace QUT.Gplib
{
    [Serializable]
    public class BufferException : Exception
    {
        public BufferException() { }
        public BufferException(string message) : base(message) { }
        public BufferException(string message, Exception innerException)
            : base(message, innerException) { }
        protected BufferException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

