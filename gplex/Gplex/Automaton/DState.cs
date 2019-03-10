// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;
using System.Collections.Generic;
using QUT.Gplex.Parser;

namespace QUT.Gplex.Automaton
{
    internal partial class DFSA
    {
        /// <summary>
        /// Class representing a DFSA state. These need to be
        /// sorted according to global ordinal number AFTER
        /// the global numbers have been allocated.
        /// </summary>
        public class DState : IComparable<DState>
        {
            static uint nextSN = 1;                // Counter for serial number allocation


            internal int rhCntx;                            // the right context fixed length
            internal int lhCntx;                            // the left context fixed length
            internal DfsaInstance myDfaInst;                // instance to which this DState belongs
            internal DFSA myDfsa;                           // a reference to the parent DFSA
            internal NSet nfaSet;               // set of nfsa state that this state represents
            internal IList<int> trList = new List<int>();    // list of transitions on this state
            internal RuleDesc accept;                       // if this is an accept state, the rule recognized
            internal string shortestStr;
            internal bool listed;
            internal bool needsBackup;

            internal int listOrd;                           // only used by the minimizer algorithm
            internal object block;                          // only used by the minimizer algorithm
            internal LinkedListNode<DState> listNode;      // only used by the minimizer algorithm
            private List<DState>[] predecessors;           // inverse nextState, only used by minimizer

            readonly uint serialNumber;                      // immutable value used in the dictionary key
            private ulong Key(int sym) { return (((ulong)serialNumber) << 22) + (uint)sym; }

            internal DState(DFSA dfsa)
            {
                Num = unset;
                serialNumber = nextSN++;
                myDfsa = dfsa;
            }

            internal DState(DfsaInstance inst)
            {
                Num = unset;
                serialNumber = nextSN++;
                myDfaInst = inst;
                myDfsa = inst.parent;
            }

            public bool HasRightContext { get { return rhCntx > 0 || lhCntx > 0; } }

            /// <summary>
            /// Final global number of this DState within whole DFSA-set. Not valid until
            /// allocation, after separation into accept and non-accept states
            /// </summary>
            public int Num
            {
                get;
                set;
            }

            /// <summary>
            /// Getter for next state transition. This wraps the dictionary access.
            /// </summary>
            /// <param name="sym">Symbol ordinal of transition</param>
            /// <returns>Next state on sym, or null</returns>
            public DState GetNext(int sym)
            {
                DState rslt;
                this.myDfsa.next.TryGetValue(this.Key(sym), out rslt);
                return rslt;
            }

            /// <summary>
            /// Enter transition in next-state dictionary
            /// </summary>
            /// <param name="sym">Symbol for transition</param>
            /// <param name="toState">Target state for transition</param>
            public void SetNext(int sym, DState toState)
            {
                //ulong key = Key(sym);
                //Console.WriteLine("inserting {0} --> {1} on {2}, key 0x{3:X12}", this.serialNumber, toState.serialNumber, sym, key);
                //DState oldState;
                //if (this.myDfsa.next.TryGetValue(key, out oldState))
                //    Console.WriteLine("Collision {0}", oldState.serialNumber);
                this.myDfsa.next.Add(this.Key(sym), toState);
            }

            /// <summary>
            /// When next state table is rewritten after minimization, this
            /// method builds the new next state dictionary that will replace
            /// the current dictionary.
            /// </summary>
            /// <param name="sym"></param>
            /// <param name="toState"></param>
            public void SetNewNext(int sym, DState toState)
            {
                this.myDfsa.newNext.Add(Key(sym), toState);
            }

            /// <summary>
            /// Getter for predecessor list for this state. Used by the 
            /// minimizer, effectively to create an inverse next-state table.
            /// </summary>
            /// <param name="ord">symbol for transitions from predecessors</param>
            /// <returns>list of states which transition to this on ord</returns>
            public IList<DState> GetPredecessors(int ord)
            {
                predecessors = predecessors ?? new List<DState>[myDfsa.MaxSym];
                if (predecessors[ord] == null)
                    predecessors[ord] = new List<DState>();
                return predecessors[ord];
            }

            public bool HasPredecessors() { return predecessors != null; }

            public bool isStart { get { return myDfaInst.start == this; } }

            /// <summary>
            /// Compare two DStates for next-state equivalence.
            /// </summary>
            /// <param name="other">state to compare with</param>
            /// <returns>predicate "next-state tables are equal"</returns>
            public bool EquivalentNextStates(DState other)
            {
                if (this.DefaultNext == other.DefaultNext &&
                    this.trList.Count == other.trList.Count)
                {
                    for (int i = 0; i < this.trList.Count; i++)
                    {
                        int sym = this.trList[i];
                        if (sym != other.trList[i] || this.GetNext(sym) != other.GetNext(sym))
                            return false;
                    }
                    return true;
                }
                else
                    return false;
            }

            /// <summary>
            /// Method to emulate full next state table from sparse data structure
            /// </summary>
            /// <param name="j"></param>
            /// <returns></returns>
            public int NextOn(int j) { return (GetNext(j) == null ? DefaultNext : GetNext(j).Num); }
            public int DefaultNext { get { return (myDfaInst == null ? DFSA.eofNum : DFSA.gotoStart); } }

            /// <summary>
            /// CompareTo method to allow sorting of DState values.
            /// </summary>
            /// <param name="r"></param>
            /// <returns></returns>
            public int CompareTo(DState r)
            {
                if (this.Num < r.Num) return -1;
                else if (this.Num > r.Num) return 1;
                else return 0;
            }

            public void AddPredecessor(DState pred, int smbl)
            {
                GetPredecessors(smbl)
                    .Add(pred);
            }

            internal void AddTrans(int ch, DState next)
            {
                SetNext(ch, next);
                trList.Add(ch);
            }

            /// <summary>
            /// Returns the name of the start condition with which this state is associated
            /// </summary>
            internal string StartConditionName
            {
                get
                {
                    if (myDfaInst != null)
                        return this.myDfaInst.myNfaInst.myStartCondition.Name;
                    else return "";
                }
            }

            internal string AbreviatedStartConditionName
            { get { string name = StartConditionName; return (name.Equals("INITIAL") ? "0" : name); } }

            /// <summary>
            /// Find the longest run of transitions with the same target
            /// in order to allow table slicing.  This must take into 
            /// account wrap around from character 'MaxSym-1' to character '\0'
            /// </summary>
            /// <param name="min">the start index of the residual table</param>
            /// <param name="rng">the length of the residual table</param>
            /// <param name="pop">the default state of the excluded run</param>
            internal void ExcludeLongestRun(out uint min, out uint rng, out int pop)
            {
                int current = NextOn(0);          // The current nextstate;
                int runLeng = 0;                  // The current run length
                int bestRun = 0;                  // Length of best run found so far.
                int bestIdx = 0;                  // Start index of remainder.
                int bestNxt = current;            // The state to exclude.
                int max = myDfsa.MaxSym;
                for (int i = 0; i < max * 2; i++) // Cater for wrap-around runs;
                {
                    int nxt = NextOn(i % max);
                    if (nxt == current)
                        runLeng++;
                    else
                    {
                        if (runLeng > bestRun)
                        {
                            bestRun = runLeng;
                            bestIdx = i;
                            bestNxt = current;
                        }
                        current = nxt;
                        runLeng = 1;
                    }
                }
                if (bestRun == max * 2)
                {
                    min = 0;
                    rng = 0;
                    pop = bestNxt;
                }
                else
                {
                    min = (uint)(bestIdx % max);
                    rng = (uint)(max - bestRun);
                    pop = bestNxt;
                }
            }

            /// <summary>
            /// Predicate "this automaton needs to implement backup moves"
            /// </summary>
            /// <returns></returns>
            internal bool NeedsBackup()
            {
                // An accept state needs backup if it has a transition to a non-accept state
                // There has to be a quicker way to do this?
                for (int i = 0; i < myDfsa.MaxSym; i++)
                    if (GetNext(i) != null && GetNext(i).accept == null) return true;
                return false;
            }

            /// <summary>
            /// Find an example character that goes off down a backup path.
            /// </summary>
            /// <returns>string denoting the character leads to a non-accept
            /// state and might need to be discarded if the match fails</returns>
            internal string BackupTransition()
            {
                int len = myDfsa.MaxSym;
                for (int i = 0; i < len; i++)
                    if (GetNext(i) != null && GetNext(i).accept == null)
                        return myDfsa.MapSymToStr(i);
                return "EOF";
            }
        }
    }
}
