// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)




namespace QUT.GPGen
{
    internal class Reduce : ParserAction
    {
        internal ProductionItem item;

        internal Reduce(ProductionItem item)
        {
            this.item = item;
        }

        public override string ToString()
        {
            return "reduce using rule " + item.production.num + " (" + item.production.lhs + ")";
        }

        internal override int ToNum()
        {
            return -item.production.num;
        }
    }
}