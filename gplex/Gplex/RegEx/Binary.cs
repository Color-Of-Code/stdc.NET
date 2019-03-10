// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    internal sealed class Binary : RegExTree
    {
        public RegExTree LeftKid { get; private set; }
        public RegExTree RightKid { get; private set; }
        internal Binary(RegOp op, RegExTree l, RegExTree r)
            : base(op)
        {
            LeftKid = l;
            RightKid = r;
        }

        internal override int ContextLength()
        {
            if (this.RightKid == null || this.LeftKid == null) return 0;
            if (Operator == RegOp.Context) throw new StringInterpretException("multiple context operators");
            else
            {
                int lLen = LeftKid.ContextLength();
                int rLen = RightKid.ContextLength();
                if (lLen <= 0 || rLen <= 0) return 0;
                else if (Operator == RegOp.Concatenation) return lLen + rLen;
                else if (lLen == rLen) return lLen;
                else return 0;
            }
        }

        internal override int MinimumLength()
        {
            if (this.RightKid == null || this.LeftKid == null) return 0;
            switch (Operator)
            {
                case RegOp.Concatenation: return LeftKid.MinimumLength() + RightKid.MinimumLength();
                case RegOp.Context: return LeftKid.MinimumLength();
                case RegOp.Alternation:
                    {
                        int lLen = LeftKid.MinimumLength();
                        int rLen = RightKid.MinimumLength();
                        return (lLen <= rLen ? lLen : rLen);
                    }
                default: throw new GplexInternalException("Bad binary RegOp");
            }
        }

        internal override bool HasRightContext
        {
            get { return Operator == RegOp.Context; }
        }

        internal override void Visit(RegExDFS visitor)
        {
            visitor.Op(this);
            LeftKid.Visit(visitor);
            RightKid.Visit(visitor);
        }
    }
}
