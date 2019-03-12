// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using QUT.Gplib;

namespace QUT.GPGen
{
    internal abstract class Symbol : ISymbol
    {
        private string _name;
        public string Kind { get; internal set; }

        // number, ordinal coding this symbol
        public abstract int Number
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

        public abstract bool IsNullable();
    }
}