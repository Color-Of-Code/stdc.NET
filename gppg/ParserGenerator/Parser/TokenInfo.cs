// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough QUT 2010
// (see accompanying GPPGcopyright.rtf)


namespace QUT.GPGen.Parser
{
    internal class TokenInfo
    {
        public string name { get; private set; }
        public string alias { get; private set; }

        // This constructor ignores explicit numeric value declarations
        // This might change later ...
        internal TokenInfo(LexSpan name, LexSpan alias)
        {
            this.name = name.ToString();
            if (alias != null)
                this.alias = alias.ToString();
        }
    }

}
