// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    internal sealed class Unary : RegExTree
    { // leftAnchor, rightAnchor, finiteRep, closure
        internal RegExTree kid;
        internal int minRep;         // min repetitions for closure/finiteRep
        internal int maxRep;         // max repetitions for finiteRep.
        internal Unary(RegOp op, RegExTree l) : base(op) { kid = l; }

        internal override int ContextLength()
        {
            if (this.kid == null) return 0;
            switch (op)
            {
                case RegOp.Closure: return 0;
                case RegOp.FiniteRepetition: return (minRep == maxRep ? kid.ContextLength() * minRep : 0);
                case RegOp.LeftAnchor: return kid.ContextLength();
                case RegOp.RightAnchor: return kid.ContextLength();
                default: throw new GplexInternalException("unknown unary RegOp");
            }
        }

        internal override int MinimumLength()
        {
            if (this.kid == null) return 0;
            switch (op)
            {
                case RegOp.Closure:
                case RegOp.FiniteRepetition: return kid.MinimumLength() * minRep;
                case RegOp.LeftAnchor:
                case RegOp.RightAnchor: return kid.MinimumLength();
                default: throw new GplexInternalException("unknown unary RegOp");
            }
        }

        internal override void Visit(RegExDFS visitor)
        {
            visitor.Op(this);
            kid.Visit(visitor);
        }

        internal override bool HasRightContext
        {
            get { return op == RegOp.LeftAnchor && kid.HasRightContext; }
        }
    }
}
