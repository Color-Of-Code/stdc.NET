// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


namespace QUT.Gplib
{
    public interface ISpan
    {
        int startLine { get; }
        int startColumn { get; }
        int endLine { get; }
        int endColumn { get; }
    }
}
