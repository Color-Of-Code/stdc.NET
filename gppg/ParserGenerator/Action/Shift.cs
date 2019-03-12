// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)


using QUT.Gplib;


namespace QUT.GPGen
{
    internal class Shift : IParserAction
    {
        internal AutomatonState next;

        internal Shift(AutomatonState next)
        {
            this.next = next;
        }

        public override string ToString()
        {
            return $"shift, and go to state {next.num}";
        }

        public int ToNum()
        {
            return next.num;
        }
    }
}