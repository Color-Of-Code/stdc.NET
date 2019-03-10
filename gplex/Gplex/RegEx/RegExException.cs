// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System;
using System.Runtime.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// Objects of this class carry exception information
    /// out to the call of Parse() on the regular expression.
    /// These exceptions cannot escape beyond the enclosing
    /// call of Parse().
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
    // Reason for FxCop message suppression -
    // This exception cannot escape from the local context
    internal class RegExException : Exception
    {
        int errNo;
        int index;
        int length;
        string text;

        internal RegExException(int errorNum, int stringIx, int count, string message) { errNo = errorNum; index = stringIx; length = count; text = message; }

        protected RegExException(SerializationInfo i, StreamingContext c) : base(i, c) { }

        internal RegExException AdjustIndex(int delta) { this.index += delta; return this; }

        internal void ListError(ErrorHandler handler, LexSpan span)
        {
            if (text == null)
                handler.ListError(span.FirstLineSubSpan(index, length), errNo);
            else
                handler.ListError(span.FirstLineSubSpan(index, length), errNo, text);
        }
    }

}
