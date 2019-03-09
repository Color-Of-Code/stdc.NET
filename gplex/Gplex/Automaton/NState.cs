// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;
using System.Collections;
using System.Collections.Generic;
using QUT.Gplex.Parser;

namespace QUT.Gplex.Automaton
{
    partial class NFSA
    {
        /// <summary>
        /// This class represents a state in the NFSA
        /// each state has an array of transitions on
        /// a particular character value, and a list 
        /// *and* bitarray of epsilon transitions.
        /// We want to go "for every epsilon do"
        /// and also do bitwise boolean ops.
        /// </summary>
        public class NState
        {
            private static uint nextSN;

            NfsaInstance myNfaInst;
            NFSA myNfsa;
            internal int ord;
            private uint serialNumber;
            internal BitArray epsilons;                 // epsilon transitions.
            internal List<NState> epsList = new List<NState>();
            internal RuleDesc accept;                   // rule matched OR null
            internal int rhCntx;                        // length of fixed right context
            internal int lhCntx;                        // length of fixed context lhs

            public NState(NfsaInstance elem)
            {
                myNfaInst = elem;
                myNfsa = elem.parent;
                serialNumber = nextSN++;
                epsilons = new BitArray(myNfaInst.MaxEps);    // Caller adds to nStates list.
            }

            internal NState GetNext(int sym)
            {
                NState rslt = null;
                ulong key = checked(((ulong)this.serialNumber << 32) + (ulong)sym);
                myNfsa.next.TryGetValue(key, out rslt);
                return rslt;
            }

            internal void SetNext(int sym, NState dstState)
            {
                ulong key = checked(((ulong)this.serialNumber << 32) + (ulong)sym);
                myNfsa.next.Add(key, dstState);
            }

            /// <summary>
            /// Add a transition from NState "this"
            /// to NState "nxt", for the character "chr".
            /// If the characters are packed, transform 
            /// from character ordinal to equivalence class 
            /// ordinal.
            /// </summary>
            /// <param name="chr">The character value</param>
            /// <param name="nxt">The destination state</param>
            public void AddChrTrns(int chr, NState nxt)
            {
                if (myNfaInst.parent.task.CaseAgnostic && chr < Char.MaxValue)
                {
                    char c = (char)chr;
                    char lo = Char.ToLower(c);
                    char hi = Char.ToUpper(c);
                    if (lo != hi)
                    {
                        AddTrns(lo, nxt);
                        AddTrns(hi, nxt);
                        return;
                    }
                }
                AddTrns(chr, nxt);
            }

            private void AddTrns(int chr, NState nxt)
            {
                if (myNfaInst.Pack)
                    chr = myNfaInst.parent.task.partition[chr];
                AddRawTransition(chr, nxt);
            }

            /// <summary>
            /// Add a transition to the NState.
            /// Assert: if the symbol ordinals are packed
            /// the mapping has already been performed
            /// </summary>
            /// <param name="ord">The symbol index</param>
            /// <param name="nxt">The destination state</param>
            private void AddRawTransition(int ord, NState nxt)
            {
                if (GetNext(ord) == null)
                    SetNext(ord, nxt);
                else        // state must have overlapping alternatives
                {
                    NState temp = myNfaInst.MkState();
                    this.AddEpsTrns(temp);
                    temp.AddRawTransition(ord, nxt);
                }
            }

            /// <summary>
            /// Add a transition from "this" to "next"
            /// for every true bit in the BitArray cls
            /// </summary>
            /// <param name="cls">The transition bit array</param>
            /// <param name="nxt">The destination state</param>
            private void AddClsTrans(BitArray cls, NState nxt)
            {
                for (int i = 0; i < cls.Count; i++)
                    if (cls[i]) AddRawTransition(i, nxt);
            }

            /// <summary>
            /// Add a transition from NState "this"
            /// to NState "nxt", for each character
            /// value in the leaf range list.
            /// If the characters are packed, transform 
            /// from character ordinal to equivalence class 
            /// ordinal.
            /// </summary>
            /// <param name="leaf">The regex leaf node</param>
            /// <param name="nxt">The destination state</param>
            public void AddClsTrans(Leaf leaf, NState nxt)
            {
                if (myNfaInst.parent.task.CaseAgnostic)
                {
                    leaf.rangeLit.list = leaf.rangeLit.list.MakeCaseAgnosticList();
                    leaf.rangeLit.list.Canonicalize();
                }

                BitArray cls = new BitArray(myNfaInst.MaxSym);
                if (myNfaInst.Pack)
                {
                    foreach (int ord in leaf.rangeLit.equivClasses)
                        cls[ord] = true;
                }
                else
                {
                    foreach (CharRange rng in leaf.rangeLit.list.Ranges)
                        for (int i = rng.minChr; i <= rng.maxChr; i++)
                            cls[i] = true;
                    if (leaf.rangeLit.list.IsInverted)
                        cls = cls.Not();
                }
                AddClsTrans(cls, nxt);
            }

            /// <summary>
            /// Add an epsilon transition from "this" to "nxt"
            /// </summary>
            /// <param name="nxt">Destination state</param>
            public void AddEpsTrns(NState nxt)
            {
                int count = epsilons.Count;
                if (count < myNfaInst.MaxEps) epsilons.Length = myNfaInst.MaxEps;
                if (!epsilons[nxt.ord])
                {
                    epsList.Add(nxt);
                    epsilons[nxt.ord] = true;
                }
            }

        }
    }
}
