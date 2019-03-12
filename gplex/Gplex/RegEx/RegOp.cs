// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    public enum RegOp
    {
        eof,
        Context,
        StringLiteral,
        Primitive,
        Concatenation, // ab
        Alternation, // a|b
        Closure, // + *
        FiniteRepetition, // {a,b}
        CharacterClass,
        LeftAnchor, // ^
        RightAnchor // $
    }
}
