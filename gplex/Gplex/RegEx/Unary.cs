// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    // LeftAnchor, RightAnchor, FiniteRepetition, Closure
    internal sealed class Unary : RegExTree
    {
        public RegExTree Kid { get; private set; }
        
        // minimum number of repetitions for Closure/FiniteRepetition
        public int MinimumOfRepetitions { get; private set; }
        
        // maximum number of repetitions for FiniteRepetition.
        public int MaximumOfRepetitions { get; private set; }
        internal Unary(RegOp op, RegExTree l, int min = 0, int max = 0)
            : base(op)
        {
            Kid = l;
            MinimumOfRepetitions = min;
            MaximumOfRepetitions = max;
        }

        internal override int ContextLength()
        {
            if (this.Kid == null) return 0;
            switch (Operator)
            {
                case RegOp.Closure:
                    return 0;
                case RegOp.FiniteRepetition:
                    return (MinimumOfRepetitions == MaximumOfRepetitions ? Kid.ContextLength() * MinimumOfRepetitions : 0);
                case RegOp.LeftAnchor:
                    return Kid.ContextLength();
                case RegOp.RightAnchor:
                    return Kid.ContextLength();
                default: throw new GplexInternalException("unknown unary RegOp");
            }
        }

        internal override int MinimumLength()
        {
            if (this.Kid == null) return 0;
            switch (Operator)
            {
                case RegOp.Closure:
                case RegOp.FiniteRepetition:
                    return Kid.MinimumLength() * MinimumOfRepetitions;
                case RegOp.LeftAnchor:
                case RegOp.RightAnchor:
                    return Kid.MinimumLength();
                default: throw new GplexInternalException("unknown unary RegOp");
            }
        }

        internal override void Visit(RegExDFS visitor)
        {
            visitor.Op(this);
            Kid.Visit(visitor);
        }

        internal override bool HasRightContext
        {
            get { return Operator == RegOp.LeftAnchor && Kid.HasRightContext; }
        }
    }
}
