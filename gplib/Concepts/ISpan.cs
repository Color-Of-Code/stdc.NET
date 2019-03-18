// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


namespace QUT.Gplib
{
    public interface ISpan
    {
        int StartLine { get; }
        int StartColumn { get; }
        int EndLine { get; }
        int EndColumn { get; }

        int startIndex { get; set; }      // start position in the buffer
        int endIndex { get; set; }        // end position in the buffer

        bool IsInitialized { get; }
    }
}
