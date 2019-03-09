// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    #region AST for Regular Expressions

    internal sealed class Unary : RegExTree
    { // leftAnchor, rightAnchor, finiteRep, closure
        internal RegExTree kid;
        internal int minRep;         // min repetitions for closure/finiteRep
        internal int maxRep;         // max repetitions for finiteRep.
        internal Unary(RegOp op, RegExTree l) : base(op) { kid = l; }

        internal override int contextLength()
        {
            if (this.kid == null) return 0;
            switch (op)
            {
                case RegOp.closure: return 0;
                case RegOp.finiteRep: return (minRep == maxRep ? kid.contextLength() * minRep : 0);
                case RegOp.leftAnchor: return kid.contextLength();
                case RegOp.rightAnchor: return kid.contextLength();
                default: throw new GplexInternalException("unknown unary RegOp");
            }
        }

        internal override int minimumLength()
        {
            if (this.kid == null) return 0;
            switch (op)
            {
                case RegOp.closure:
                case RegOp.finiteRep: return kid.minimumLength() * minRep;
                case RegOp.leftAnchor:
                case RegOp.rightAnchor: return kid.minimumLength();
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
            get { return op == RegOp.leftAnchor && kid.HasRightContext; }
        }
    }
    #endregion
}
