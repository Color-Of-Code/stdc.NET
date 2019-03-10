// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

namespace QUT.GPGen
{
    internal abstract class Symbol
    {
        private string _name;
        internal string kind;

        // number, ordinal coding this symbol
        internal abstract int Number
        {
            get;
        }

        internal Symbol(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }

        internal abstract bool IsNullable();
    }
}