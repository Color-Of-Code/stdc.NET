// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)




namespace QUT.GPGen
{
    internal class Shift : ParserAction
    {
        internal AutomatonState next;

        internal Shift(AutomatonState next)
        {
            this.next = next;
        }

        public override string ToString()
        {
            return "shift, and go to state " + next.num;
        }

        internal override int ToNum()
        {
            return next.num;
        }
    }
}