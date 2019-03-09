// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    #region AST for Regular Expressions

    /// <summary>
    /// Abstract base class for depth first
    /// search visitor on regular expressions.
    /// </summary>
    internal abstract class RegExDFS
    {
        internal abstract void Op(RegExTree tree);
    }
    #endregion
}
