// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough QUT 2010
// (see accompanying GPPGcopyright.rtf)

using QUT.Gplib;

namespace QUT.GPGen.Parser
{
    internal class ActionProxy
    {
        // TODO: setters should become better encapsulated
        public ISpan codeBlock { get; private set; }
        public ISpan precedenceToken { get; set; }
        public ISpan precedenceSpan { get; set; }

        internal ActionProxy(ISpan precedence, ISpan identifier, ISpan code)
        {
            codeBlock = code;
            precedenceToken = identifier;
            precedenceSpan = precedence;
        }
    }

}
