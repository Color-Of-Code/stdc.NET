// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough QUT 2010
// (see accompanying GPPGcopyright.rtf)

using QUT.Gplib;

namespace QUT.GPGen.Parser
{
    internal class TokenInfo : IToken
    {
        public string Name { get; private set; }
        public string Alias { get; private set; }

        // This constructor ignores explicit numeric value declarations
        // This might change later ...
        internal TokenInfo(ISpan name, ISpan alias)
        {
            this.Name = name.ToString();
            if (alias != null)
                this.Alias = alias.ToString();
        }
    }

}
