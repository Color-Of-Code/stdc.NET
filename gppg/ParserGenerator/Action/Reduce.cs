// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)


using QUT.Gplib;

namespace QUT.GPGen
{
    internal class Reduce : IParserAction
    {
        internal ProductionItem item;

        internal Reduce(ProductionItem item)
        {
            this.item = item;
        }

        public override string ToString()
        {
            return $"reduce using rule {item.production.num} ({item.production.lhs})";
        }

        public int ToNum()
        {
            return -item.production.num;
        }
    }
}