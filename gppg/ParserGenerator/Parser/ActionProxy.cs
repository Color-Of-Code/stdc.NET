// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough QUT 2010
// (see accompanying GPPGcopyright.rtf)

using QUT.Gplib;

namespace QUT.GPGen.Parser
{
    internal class ActionProxy
    {
        // TODO: setters should become better encapsulated
        public LexSpan codeBlock { get; set; }
        public LexSpan precedenceToken { get; set; }
        public LexSpan precedenceSpan { get; set; }

        internal ActionProxy(LexSpan precedence, LexSpan identifier, LexSpan code)
        {
            codeBlock = code;
            precedenceToken = identifier;
            precedenceSpan = precedence;
        }
    }

}
