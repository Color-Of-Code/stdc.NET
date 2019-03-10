// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System;
using System.Collections.Generic;
using System.Globalization;

namespace QUT.Gplex.Parser
{
    internal sealed class RuleDesc
    {
        static int next = 1;
        string pattern;       // Span of pattern reg-exp
        int minPatternLength;
        internal LexSpan pSpan;
        internal int ord;
        RegExTree reAST;
        internal LexSpan aSpan; // Span of lexical action
        internal bool isBarAction;
        //internal bool isRightAnchored;
        internal bool isPredDummyRule;
        internal IList<StartState> list;

        /// <summary>
        /// How many times this rule is used
        /// in the construction of the DFSA. 
        /// May decrease due to replacement.
        /// </summary>
        internal int useCount;
        /// <summary>
        /// If rule is replaced, the new rule
        /// that replaced this one.
        /// </summary>
        internal RuleDesc replacedBy;

        internal string Pattern { get { return pattern; } }

        /// <summary>
        /// A pattern is a loop risk iff:
        /// (1) it can match the empty string,
        /// (2) it has non-zero right context.
        /// Condition (1) alone will not cause a loop, since
        /// any non-zero length match will take precedence.
        /// </summary>
        internal bool IsLoopRisk
        {
            get
            {
                return
                    pSpan != null &&
                    reAST != null &&
                    minPatternLength == 0 &&
                    reAST.HasRightContext;
            }
        }

        private RuleDesc() { }

        internal RuleDesc(LexSpan loc, LexSpan act, IList<StartState> aList, bool bar)
        {
            pSpan = loc;
            aSpan = act;
            pattern = pSpan.buffer.GetString(pSpan.startIndex, pSpan.endIndex);
            isBarAction = bar;
            list = aList;
            ord = next++;
        }

        internal static RuleDesc MkDummyRuleDesc(LexCategory cat, AAST aast)
        {
            RuleDesc result = new RuleDesc();
            result.pSpan = null;
            result.aSpan = aast.AtStart;
            result.isBarAction = false;
            result.isPredDummyRule = true;
            result.pattern = String.Format(CultureInfo.InvariantCulture, "{{{0}}}", cat.Name);
            result.list = new List<StartState>();
            result.ParseRE(aast);
            result.list.Add(aast.StartStateValue(cat.PredDummyName));
            return result;
        }

        internal RegExTree Tree { get { return reAST; } }
        internal bool hasAction { get { return aSpan.IsInitialized; } }
        // internal void Dump() { Console.WriteLine(pattern); }

        internal void ParseRE(AAST aast)
        {
            reAST = new AAST.ReParser(pattern, pSpan, aast).Parse();
            SemanticCheck(aast);
        }

        /// <summary>
        /// This is the place to perform any semantic checks on the 
        /// trees corresponding to a rule of the LEX grammar,
        /// during a recursive traversal of the tree.  It is hard
        /// to do these on the fly during AST construction, because
        /// of the tree-grafting that happens for lexical categories.
        /// 
        /// First check is that '^' and '$' can only appear 
        /// (logically) at the ends of the pattern.
        /// Later need to check ban on multiple right contexts ...
        /// </summary>
        /// <param name="aast"></param>
        void SemanticCheck(AAST aast)
        {
            RegExTree tree = reAST;
            if (tree != null && tree.op == RegOp.leftAnchor) tree = ((Unary)tree).kid;
            if (tree != null && tree.op == RegOp.rightAnchor)
            {
                tree = ((Unary)tree).kid;
                if (tree.op == RegOp.context)
                    aast.hdlr.ListError(pSpan, 100);
            }
            Check(aast, tree);
            if (tree != null)
                minPatternLength = tree.MinimumLength();
        }

        void Check(AAST aast, RegExTree tree)
        {
            Binary bnryTree;
            Unary unryTree;

            if (tree == null) return;
            switch (tree.op)
            {
                case RegOp.charClass:
                case RegOp.primitive:
                case RegOp.litStr:
                case RegOp.eof:
                    break;
                case RegOp.context:
                case RegOp.concat:
                case RegOp.alt:
                    bnryTree = (Binary)tree;
                    Check(aast, bnryTree.lKid);
                    Check(aast, bnryTree.rKid);
                    if (tree.op == RegOp.context &&
                        bnryTree.lKid.ContextLength() == 0 &&
                        bnryTree.rKid.ContextLength() == 0) aast.hdlr.ListError(pSpan, 75);
                    break;
                case RegOp.closure:
                case RegOp.finiteRep:
                    unryTree = (Unary)tree;
                    Check(aast, unryTree.kid);
                    break;
                case RegOp.leftAnchor:
                case RegOp.rightAnchor:
                    aast.hdlr.ListError(pSpan, 69);
                    break;
            }
        }
    }

}
