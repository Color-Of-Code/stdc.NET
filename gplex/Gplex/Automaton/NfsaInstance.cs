// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System.Collections.Generic;
using QUT.Gplex.Parser;
using QUT.Gplib;

namespace QUT.Gplex.Automaton
{
    partial class NFSA
    {
        /// <summary>
        /// This class represents the NFSA for all the regular expressions that 
        /// begin on a given start condition. Each NfsaInstance may have two "start"
        /// states, corresponding to the anchored and unanchored pattern starts.
        /// </summary>
        public class NfsaInstance
        {
            const int defN = 32;
            internal string key;
            internal LexSpan eofAction;
            internal StartState myStartCondition;           // from the LEX start state
            internal IList<int> acceptStates = new List<int>();
            internal IList<NState> nStates = new List<NState>();
            internal NFSA parent;

            private bool pack;
            private int maxE = defN;                       // number of elements in epsilon BitArray
            private int maxS;

            NState anchorState;
            NState entryState;

            public NfsaInstance(StartState ss, NFSA parent)
            {
                myStartCondition = ss;
                this.parent = parent;
                this.pack = parent.task.ChrClasses;
                if (pack)
                    maxS = parent.task.partition.Length;          // Number of equivalence classes
                else
                    maxS = parent.task.TargetSymCardinality;      // Size of alphabet
                entryState = MkState();
            }

            /// <summary>
            /// The target alphabet cardinality
            /// </summary>
            internal int MaxSym { get { return maxS; } }

            /// <summary>
            /// True means this automaton uses character equivalence classes
            /// </summary>
            internal bool Pack { get { return pack; } }

            /// <summary>
            /// The current size of the dynamically sized epsilon list
            /// </summary>
            internal int MaxEps { get { return maxE; } }

            internal bool LeftAnchored { get { return anchorState != null; } }

            internal NState Entry { get { return entryState; } }

            internal NState MkState()
            {
                NState s = new NState(this);
                s.ord = nStates.Count;
                if (s.ord >= maxE) maxE *= 2;
                nStates.Add(s);
                return s;
            }

            internal NState MkState(NState entryState)
            {
                NState s = MkState();
                s.AddEpsTrns(entryState);
                return s;
            }

            /// <summary>
            /// NfsaInst objects only have an anchorState if there is one or more
            /// left-anchored patterns for the corresponding start condition.
            /// AnchorState is allocated on demand when the first such pattern
            /// is discoverd during MakePath.
            /// </summary>
            internal NState AnchorState
            {
                get
                {
                    anchorState = anchorState ?? MkState(entryState);
                    return anchorState;
                }
            }


            internal void MarkAccept(NState acpt, RuleDesc rule)
            {
                acpt.accept = rule;
                acceptStates.Add(acpt.ord);
            }

            /// <summary>
            /// Create a transition path in the NFSA from the given 
            /// start state to the given end state, corresponding to the
            /// RegEx tree value.  The method may (almost always does)
            /// create new NFSA states and recurses to make paths for
            /// the subtrees of the given tree.
            /// </summary>
            /// <param name="tree">The tree to encode</param>
            /// <param name="start">The start state for the pattern</param>
            /// <param name="end">The end state for the pattern</param>
            internal void MakePath(RegExTree tree, NState startState, NState endState)
            {
                NState tmp1 = null;
                NState tmp2 = null;
                int rLen, lLen;

                switch (tree.op)
                {
                    case RegOp.eof:
                        break;
                    // Binary nodes ===================================
                    case RegOp.Context:
                    case RegOp.Concatenation:
                    case RegOp.Alternation:
                        // Binary nodes ===================================
                        Binary binNode = tree as Binary;
                        switch (tree.op)
                        {
                            case RegOp.Context:
                                rLen = binNode.rKid.ContextLength();
                                lLen = binNode.lKid.ContextLength();
                                if (rLen <= 0 && lLen <= 0)
                                    throw new StringInterpretException("variable right context '/' not implemented");
                                else
                                {
                                    endState.rhCntx = rLen;
                                    endState.lhCntx = lLen;
                                    tmp1 = MkState();
                                    MakePath(binNode.lKid, startState, tmp1);
                                    MakePath(binNode.rKid, tmp1, endState);
                                }
                                break;
                            case RegOp.Concatenation:
                                tmp1 = MkState();
                                MakePath(binNode.lKid, startState, tmp1);
                                MakePath(binNode.rKid, tmp1, endState);
                                break;
                            case RegOp.Alternation:
                                tmp1 = MkState();
                                MakePath(binNode.lKid, startState, tmp1);
                                tmp1.AddEpsTrns(endState);
                                tmp1 = MkState();
                                MakePath(binNode.rKid, startState, tmp1);
                                tmp1.AddEpsTrns(endState);
                                break;
                        }
                        break;
                    // Unary nodes ===================================
                    case RegOp.Closure:
                    case RegOp.FiniteRepetition:
                        // Unary nodes ===================================
                        Unary unaryNode = tree as Unary;
                        switch (tree.op)
                        {
                            case RegOp.Closure:
                                tmp2 = MkState();
                                if (unaryNode.minRep == 0)
                                {
                                    tmp1 = MkState();
                                    startState.AddEpsTrns(tmp1);
                                }
                                else
                                {
                                    NState dummy = startState;
                                    for (int i = 0; i < unaryNode.minRep; i++)
                                    {
                                        tmp1 = MkState();
                                        MakePath(unaryNode.kid, dummy, tmp1);
                                        dummy = tmp1;
                                    }
                                }
                                MakePath(unaryNode.kid, tmp1, tmp2);
                                tmp2.AddEpsTrns(tmp1);
                                tmp1.AddEpsTrns(endState);
                                break;
                            case RegOp.FiniteRepetition:
                                {
                                    NState dummy = tmp1 = startState;
                                    for (int i = 0; i < unaryNode.minRep; i++)
                                    {
                                        tmp1 = MkState();
                                        MakePath(unaryNode.kid, dummy, tmp1);
                                        dummy = tmp1;
                                    }
                                    tmp1.AddEpsTrns(endState);
                                    for (int i = unaryNode.minRep; i < unaryNode.maxRep; i++)
                                    {
                                        tmp1 = MkState();
                                        MakePath(unaryNode.kid, dummy, tmp1);
                                        dummy = tmp1;
                                        dummy.AddEpsTrns(endState);
                                    }
                                }
                                break;
                        }
                        break;
                    // Leaf nodes ===================================
                    case RegOp.StringLiteral:
                    case RegOp.Primitive:
                    case RegOp.CharacterClass:
                        // Leaf nodes ===================================
                        Leaf leafNode = tree as Leaf;
                        switch (tree.op)
                        {
                            case RegOp.StringLiteral:
                                {
                                    // Make a linear sequence of states with successive
                                    // transitions on successive string characters.
                                    //
                                    string text = leafNode.str;
                                    NState dummy = startState;
                                    // Need to deal with special case of empty string
                                    if (text.Length == 0)
                                        dummy.AddEpsTrns(endState);
                                    else
                                    {
                                        //  This code is complicated by the fact that unicode
                                        //  escape substitution may have inserted surrogate
                                        //  pairs of characters in the string.  We need
                                        //  one transition for every unicode codepoint,
                                        //  not one for every char value in this string.
                                        //
                                        int index = 0;
                                        int code = CharacterUtilities.CodePoint(text, ref index); // First character
                                        int next = CharacterUtilities.CodePoint(text, ref index); // Next, possibly -1
                                        while (next >= 0)
                                        {
                                            tmp1 = MkState();
                                            dummy.AddChrTrns(code, tmp1);
                                            dummy = tmp1;
                                            code = next;
                                            next = CharacterUtilities.CodePoint(text, ref index);
                                        }
                                        // Postcondition ==> "code" is the last char.
                                        dummy.AddChrTrns(code, endState);
                                    }
                                }
                                break;
                            case RegOp.Primitive:
                                startState.AddChrTrns(leafNode.chVal, endState);
                                break;

                            case RegOp.CharacterClass:
                                startState.AddClsTrans(leafNode, endState);
                                break;
                        }
                        break;
                    default: throw new GplexInternalException("unknown tree op");
                }
            }

        }
    }
}
