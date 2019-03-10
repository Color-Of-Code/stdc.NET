// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    internal class Leaf : RegExTree
    {   // charClass, EOF, litStr, primitive

        internal int chVal;     // in case of primitive char
        internal string str;
        internal RangeLiteral rangeLit;

        internal Leaf(string s) : base(RegOp.StringLiteral) { str = s; } // Don't reinterpret escapes.
        internal Leaf(int code) : base(RegOp.Primitive) { chVal = code; }
        internal Leaf(RegOp op) : base(op) { }


        internal override int ContextLength()
        {
            return (op == RegOp.StringLiteral ? this.str.Length : 1);
        }

        internal override int MinimumLength() { return ContextLength(); }

        internal override void Visit(RegExDFS visitor) { visitor.Op(this); }

        internal void Merge(Leaf addend)
        {
            foreach (CharRange rng in addend.rangeLit.list.Ranges)
                this.rangeLit.list.Add(rng);
        }

        internal void Subtract(Leaf subtrahend)
        {
            this.rangeLit.list = this.rangeLit.list.SUB(subtrahend.rangeLit.list);
        }

        internal void Intersect(Leaf rhOperand)
        {
            this.rangeLit.list = this.rangeLit.list.AND(rhOperand.rangeLit.list);
        }
    }
}
