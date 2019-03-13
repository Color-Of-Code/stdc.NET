// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using QUT.Gplib;

namespace QUT.GPGen
{
    public abstract class Symbol : ISymbol
    {
        public string Kind
        { get; internal set; }

        public abstract int Id
        { get; }

        public string Name
        { get; private set; }

        protected Symbol(string name)
            => Name = name;

        public override string ToString()
            => Name;

        public abstract bool IsNullable();

        public abstract bool IsTerminating();
    }
}