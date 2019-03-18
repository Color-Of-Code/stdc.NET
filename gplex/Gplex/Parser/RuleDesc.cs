// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System;
using System.Collections.Generic;
using System.Globalization;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    internal sealed class RuleDesc
    {
        private static IdGenerator _ids = new IdGenerator(1);
        int minPatternLength;
        internal LexSpan pSpan { get; private set; }
        internal int ord { get; private set; }
        RegExTree reAST;
        internal ISpan aSpan { get; set; } // Span of lexical action
        internal bool isBarAction;
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

        // Span of pattern reg-exp
        internal string Pattern { get; private set; }

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
            Pattern = pSpan.buffer.GetString(pSpan.startIndex, pSpan.endIndex);
            isBarAction = bar;
            list = aList;
            ord = _ids.Next();
        }

        internal static RuleDesc MkDummyRuleDesc(LexCategory cat, AAST aast)
        {
            RuleDesc result = new RuleDesc();
            result.pSpan = null;
            result.aSpan = aast.AtStart;
            result.isBarAction = false;
            result.isPredDummyRule = true;
            result.Pattern = String.Format(CultureInfo.InvariantCulture, "{{{0}}}", cat.Name);
            result.list = new List<StartState>();
            result.ParseRE(aast);
            result.list.Add(aast.StartStateValue(cat.PredDummyName));
            return result;
        }

        internal RegExTree Tree { get { return reAST; } }
        internal bool HasAction { get { return aSpan.IsInitialized; } }

        internal void ParseRE(AAST aast)
        {
            reAST = new AAST.RegularExpressionParser(Pattern, pSpan, aast).Parse();
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
            if (tree != null && tree.Operator == RegOp.LeftAnchor) tree = ((Unary)tree).Kid;
            if (tree != null && tree.Operator == RegOp.RightAnchor)
            {
                tree = ((Unary)tree).Kid;
                if (tree.Operator == RegOp.Context)
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
            switch (tree.Operator)
            {
                case RegOp.CharacterClass:
                case RegOp.Primitive:
                case RegOp.StringLiteral:
                case RegOp.eof:
                    break;
                case RegOp.Context:
                case RegOp.Concatenation:
                case RegOp.Alternation:
                    bnryTree = (Binary)tree;
                    Check(aast, bnryTree.LeftKid);
                    Check(aast, bnryTree.RightKid);
                    if (tree.Operator == RegOp.Context &&
                        bnryTree.LeftKid.ContextLength() == 0 &&
                        bnryTree.RightKid.ContextLength() == 0) aast.hdlr.ListError(pSpan, 75);
                    break;
                case RegOp.Closure:
                case RegOp.FiniteRepetition:
                    unryTree = (Unary)tree;
                    Check(aast, unryTree.Kid);
                    break;
                case RegOp.LeftAnchor:
                case RegOp.RightAnchor:
                    aast.hdlr.ListError(pSpan, 69);
                    break;
            }
        }
    }
}
