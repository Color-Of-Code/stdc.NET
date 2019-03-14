// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System.Collections.Generic;
using System.Linq;
using QUT.Gplex.Parser;
using QUT.Gplib;

namespace QUT.Gplex.Automaton
{
    /// <summary>
    /// This nested class represents all of the transitions starting
    /// from a particular StartCondition. There is usually one start
    /// state only.  However, if this particular start condition has
    /// one or more left-anchored pattern, then there will be an
    /// anchor start state as well.
    /// </summary>
    public class DfsaInstance : IStateMachine
    {
        internal int instNext;                     // number of next state to be allocated
        internal int acceptCount;
        internal DState start;
        internal DState anchor;
        internal DFSA parent;                      // Parent DFSA reference
        internal LexSpan eofCode;                  // Text span for EOF semantic action.
        internal NfsaInstance myNfaInst;      // Corresponding NFSA instance
        NSetFactory factory;                       // Factory for creating NSet objects

        private IDictionary<NSet, DState> dfaTable = new Dictionary<NSet, DState>();

        internal DfsaInstance(NfsaInstance nfa, DFSA dfa)
        {
            myNfaInst = nfa;
            parent = dfa;
            eofCode = nfa.eofAction;
            factory = new NSetFactory(myNfaInst.MaxEps);
        }

        public int StartConditionOrd { get { return myNfaInst.myStartCondition.Ord; } }
        public string StartConditionName { get { return myNfaInst.myStartCondition.Name; } }

        /// <summary>
        /// Convert the NFSA for this particular StartCondition.
        /// This uses the classical subset construction algorithm.
        /// </summary>
        internal void Convert()
        {
            var stack = new Stack<DState>();
            NSet sSet = factory.MkNewSet();
            int symCardinality = parent.MaxSym;
            sSet.Insert(myNfaInst.EntryState.ord);
            MkClosure(sSet);
            start = MkNewState(sSet);
            stack.Push(start);
            if (myNfaInst.LeftAnchored)
            {
                //  The NfsaInst flag shows that the corresponding start
                //  condition has at least one rule that is left-anchored.
                //  This means that this DfsaInst must have two start
                //  states. One is the usual "this.start" plus another
                //  "this.anchor" for patterns starting in column 0.
                //  The whole automaton must use a slower next-state loop.
                parent.hasLeftAnchors = true;
                sSet = factory.MkNewSet();
                sSet.Insert(myNfaInst.AnchorState.ord);
                MkClosure(sSet);
                anchor = MkNewState(sSet);
                stack.Push(anchor);
            }
            // Next is the worklist algorithm.  Newly created dfsa states
            // are placed on the stack.  When popped the next states
            // are computed from the nfsa transition information.
            while (stack.Any())
            {
                DState last = stack.Pop();
                NSet pSet = last.nfaSet;
                //
                // For this state we are going to consider every
                // possible transition, each input symbol at a time.
                //
                for (int ch = 0; ch < symCardinality; ch++)                 // For every character do
                {
                    // For each transition out of "last" we
                    // will form a set of NFSA states and find
                    // or create a corresponding DFSA state.
                    NSet nxSet = null;
                    DState nxState = null;
                    NEnumerator inum = pSet.GetEnumerator();
                    while (inum.MoveNext())    // foreach NFSA state contained in "last" 
                    {
                        int i = inum.Current;
                        NState nStI = myNfaInst.nStates[i];   // get the nfsa state
                        NState nStCh = nStI.GetNext(ch);      // get the nfsa next state
                        if (nStCh != null)                         // ==> we have a transition
                        {
                            // Create next state set on demand, and insert ord in set.
                            nxSet = nxSet ?? factory.MkNewSet();
                            nxSet.Insert(nStCh.ord);
                        }
                    }
                    // If nxSet is not null, then there must have been
                    // at least one transition on the current symbol ch.
                    if (nxSet != null)
                    {
                        // NSetFactory.NSet save = nxSet;

                        // Enhance the state set with all the
                        // states in the epsilon-closure. We then
                        // look up the set in the dictionary.  If
                        // the set is in the dictionary retrieve 
                        // the corresponding DState, else create
                        // a new DState and add it to the worklist.
                        MkClosure(nxSet);
                        if (dfaTable.ContainsKey(nxSet))
                            nxState = dfaTable[nxSet];
                        if (nxState == null)
                        {
                            // Console.WriteLine("nxSet, <{0}, {1}>", save.Diag(), nxSet.Diag());
                            nxState = MkNewState(nxSet);
                            stack.Push(nxState);
                        }
                        last.AddTransition(ch, nxState);
                    }
                }
            }
        }

        /// <summary>
        /// Create a new DState corresponding to the 
        /// given set of NFSA ordinal numbers.
        /// </summary>
        /// <param name="stateSet">The set of NFSA states</param>
        /// <returns>The new state</returns>
        internal DState MkNewState(NSet stateSet)
        {
            DState dSt = new DState(this);
            dSt.nfaSet = stateSet;
            instNext++;

            parent.stateList.Add(dSt);
            dfaTable.Add(stateSet, dSt);

            // Console.WriteLine("New DState, <{0}>", stateSet.Diag());

            foreach (int i in myNfaInst.acceptStates)
                if (stateSet.Contains(i))
                {
                    NState nfas = myNfaInst.nStates[i];
                    RuleDesc rule = nfas.accept;
                    if (dSt.Num == DFSA.unset)
                    {
                        // DFSA accept states are compact in the numbering
                        dSt.Num = parent.globNext++;
                        acceptCount++;
                    }
                    if (dSt.accept == null || rule.ord < dSt.accept.ord)
                    {
                        if (dSt.accept != null)
                        {
                            //  Rule "rule" is about to override rule "dSt.accept"
                            //List<RuleDesc> list = parent.myTask.aast.ruleList;
                            //string ruleStr = rule.Pattern;
                            //string oldStr = dSt.accept.Pattern;
                            //int count = dSt.accept.useCount;

                            dSt.accept.useCount--;
                            dSt.accept.replacedBy = rule;
                        }
                        rule.useCount++;
                        dSt.accept = rule;
                        dSt.rhCntx = (nfas.rhCntx > 0 ? nfas.rhCntx : 0);
                        dSt.lhCntx = (nfas.lhCntx > 0 ? nfas.lhCntx : 0);
                    }
                }
            return dSt;
        }


        /// <summary>
        /// The epsilon closure algorithm
        /// The set passed in as argument is updated in place.
        /// </summary>
        /// <param name="states">initial set of NFSA states</param>
        /// <returns>updated set of NFSA states</returns>
        internal void MkClosure(NSet set)
        {
            var stack = new Stack<int>();
            NEnumerator inum = set.GetEnumerator();
            while (inum.MoveNext()) stack.Push(inum.Current);
            while (stack.Any())
            {
                int pos = stack.Pop();
                foreach (NState nxt in myNfaInst.nStates[pos].epsList)
                {
                    if (!set.Contains(nxt.ord)) { set.Insert(nxt.ord); stack.Push(nxt.ord); }
                }
            }
        }
    }
}
