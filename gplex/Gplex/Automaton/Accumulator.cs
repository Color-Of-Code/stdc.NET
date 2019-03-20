// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// This is the visitor pattern that extracts
    /// ranges from the leaves of the Regular Expressions.
    /// The RegExDFS base class provides the traversal
    /// code, while this extension provides the "Op" method.
    /// </summary>
    internal class Accumulator : RegExDFS
    {
        Partition partition;

        internal Accumulator(Partition part) { this.partition = part; }

        // Refine based on a single, isolated character.
        // The singleton is transformed into a RangeLiteral with just
        // one element in its RangeList, with a one-length range. 
        private void DoSingleton(int ch)
        {
            if (this.partition.myTask.CaseAgnostic)
                partition.Refine(RangeLiteral.NewCaseAgnosticPair(ch));
            else
                partition.Refine(new RangeLiteral(ch));
        }

        private void DoLiteral(RangeLiteral lit)
        {
            if (lit.lastPart != this.partition)
            {
                lit.lastPart = this.partition;
                if (this.partition.myTask.CaseAgnostic)
                    lit.list = lit.list.MakeCaseAgnosticList();
                lit.list.Canonicalize();
                partition.Refine(lit);
            }
        }

        internal override void Op(RegExTree tree)
        {
            Leaf leaf = tree as Leaf;
            if (leaf != null)
            {
                switch (leaf.Operator)
                {
                    case RegOp.Primitive:
                        DoSingleton(leaf.chVal);
                        break;
                    case RegOp.StringLiteral:
                        for (int index = 0; ;)
                        {
                            // Use CodePoint in case string has surrogate pairs.
                            int code = CharacterUtilities.CodePoint(leaf.str, ref index);
                            if (code == -1)
                                break;
                            else
                                DoSingleton(code);
                        }
                        break;
                    case RegOp.CharacterClass:
                        DoLiteral(leaf.rangeLit);
                        break;
                    case RegOp.eof: // no action required
                        break;
                    default:
                        throw new ToolInternalException("Unknown RegOp");
                }
            }
            else if (tree.Operator == RegOp.RightAnchor)
                DoLiteral(RangeLiteral.RightAnchors);
        }
    }
}
