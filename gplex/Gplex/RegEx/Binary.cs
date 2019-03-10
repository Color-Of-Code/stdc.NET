// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    internal sealed class Binary : RegExTree
    {
        internal RegExTree lKid, rKid;
        internal Binary(RegOp op, RegExTree l, RegExTree r) : base(op) { lKid = l; rKid = r; }

        internal override int ContextLength()
        {
            if (this.rKid == null || this.lKid == null) return 0;
            if (op == RegOp.context) throw new StringInterpretException("multiple context operators");
            else
            {
                int lLen = lKid.ContextLength();
                int rLen = rKid.ContextLength();
                if (lLen <= 0 || rLen <= 0) return 0;
                else if (op == RegOp.concat) return lLen + rLen;
                else if (lLen == rLen) return lLen;
                else return 0;
            }
        }

        internal override int MinimumLength()
        {
            if (this.rKid == null || this.lKid == null) return 0;
            switch (op)
            {
                case RegOp.concat: return lKid.MinimumLength() + rKid.MinimumLength();
                case RegOp.context: return lKid.MinimumLength();
                case RegOp.alt:
                    {
                        int lLen = lKid.MinimumLength();
                        int rLen = rKid.MinimumLength();
                        return (lLen <= rLen ? lLen : rLen);
                    }
                default: throw new GplexInternalException("Bad binary RegOp");
            }
        }

        internal override bool HasRightContext
        {
            get { return op == RegOp.context; }
        }

        internal override void Visit(RegExDFS visitor)
        {
            visitor.Op(this);
            lKid.Visit(visitor);
            rKid.Visit(visitor);
        }
    }
}
