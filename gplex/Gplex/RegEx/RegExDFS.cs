// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    /// <summary>
    /// Abstract base class for depth first
    /// search visitor on regular expressions.
    /// </summary>
    internal abstract class RegExDFS
    {
        internal abstract void Op(RegExTree tree);
    }
}
