// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    #region AST for Regular Expressions

    internal enum RegOp
    {
        eof,
        context,
        litStr,
        primitive,
        concat,
        alt,
        closure,
        finiteRep,
        charClass,
        leftAnchor,
        rightAnchor
    }
    #endregion
}
