// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace QUT.Gplex.Automaton
{
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Minimizer")]
    // Apparently "minimizer" is not in the FxCop dictionary?  It should be!
    public class Minimizer
    {
        private DFSA dfsa;
        private Stack<PartitionBlock> worklist = new Stack<PartitionBlock>();

        PartitionBlock otherStates;
        PartitionBlock startStates;
        List<PartitionBlock> acceptStates = new List<PartitionBlock>();
        List<PartitionBlock> allBlocks = new List<PartitionBlock>();

        internal Minimizer(DFSA dfsa)
        {
            this.dfsa = dfsa;
            otherStates = MkNewBlock();
            startStates = MkNewBlock();
        }

        PartitionBlock MkNewBlock()
        {
            PartitionBlock blk = new PartitionBlock(dfsa.MaxSym);
            allBlocks.Add(blk);
            return blk;
        }

        /// <summary>
        /// The initial partitions are formed by the following
        /// rules: 
        /// Every accept state shares a partition with all other
        /// accept states that have the same semantic action. 
        /// All other states go in the partition "otherStates".
        /// 
        /// Addendum, Version 0.3: start states must be kept out
        /// of the otherStates partition, otherwise prefixes can
        /// be chopped off by, for example, concluding that states
        /// {1,3} are equivalent in the following FSA.  They are,
        /// but the computation of yytext is broken if you merge!
        /// 
        ///                 a     b     c     d
        /// (start state) 1---->2---->3---->4-----(5) (accept state)
        /// 
        ///                  c     d
        /// (start state) 1---->7---->(5) (accept state)
        /// 
        /// The inverse transition function is computed at the same
        /// time as the initial partition is performed.
        /// </summary>
        /// <param name="list">The list of all DStates</param>
        internal void PopulatePartitions(List<DFSA.DState> list)
        {
            PartitionBlock blk = null;
            dfsa.origLength = list.Count;
            for (int i = 1; i < list.Count; i++)
            {
                DFSA.DState dSt = list[i];
                if (dSt.accept != null)
                    blk = FindPartition(dSt);
                else if (dSt.isStart)
                    blk = startStates;
                else
                    blk = otherStates;
                blk.AddState(dSt);
                dSt.block = blk;
                // now create the inverse transition function
                for (int j = 0; j < dfsa.MaxSym; j++)
                {
                    DFSA.DState pred = dSt.GetNext(j);
                    if (pred != null) pred.AddPredecessor(dSt, j);
                }
            }
            // Now add the eofState as an accept state.
            // This is *despite* the state not having an accept reference!
            blk = MkNewBlock();
            acceptStates.Add(blk);
            blk.AddState(list[0]);
            list[0].block = blk;
            // And now finally initialize the pair list.
            worklist.Push(startStates);
            worklist.Push(otherStates); // EXPERIMENTAL
            foreach (PartitionBlock lst in acceptStates) worklist.Push(lst);
        }


        /// <summary>
        /// Find an existing partition block with which dSt is compatible,
        /// or construct a new partition into which dSt can be placed.
        /// </summary>
        /// <param name="dSt"></param>
        /// <returns></returns>
        PartitionBlock FindPartition(DFSA.DState dSt)
        {
            foreach (PartitionBlock blk in acceptStates)
            {
                //  Assert every partition in acceptStates has at least one member.
                //  Assert every member has the same semantic action.
                //
                //  This would be a simple matter except for right context. Is such
                //  cases the action is an input backup, then the user action. For a 
                //  pattern R1/R2 the regex R1.R2 (concatenation) is recognized
                //  and the buffer backed up to the position of the '/'.
                //  In the case of R1 of fixed length N we do "yyless(N);"
                //  In the case of R2 of fixed length N we do "yyless(yyleng-N);"
                //  If the first state in the partition has both lengths fixed
                //  we must choose one or the other backup action, and only add 
                //  other states that are compatible with that choice.
                DFSA.DState first = blk.FirstMember;
                if (DFSA.SpansEqual(first.accept.aSpan, dSt.accept.aSpan))
                    if (!first.HasRightContext && !dSt.HasRightContext)
                        return blk;
                    else
                    {
                        if (first.lhCntx > 0 && first.lhCntx == dSt.lhCntx)
                        // From now on only add states with matching lhs length
                        { first.rhCntx = 0; dSt.rhCntx = 0; return blk; }
                        if (first.rhCntx > 0 && first.rhCntx == dSt.rhCntx)
                        // From now on only add states with matching rhs length
                        { first.lhCntx = 0; dSt.lhCntx = 0; return blk; }
                    }
            }
            PartitionBlock nxt = MkNewBlock();
            acceptStates.Add(nxt);
            return nxt;
        }

        /// <summary>
        /// Maps old dfsa states to new states in the minimized set.
        /// </summary>
        /// <param name="dSt">The state to be mapped</param>
        /// <returns>The replacement state</returns>
        internal static DFSA.DState PMap(DFSA.DState dSt)
        {
            PartitionBlock blk = dSt.block as PartitionBlock;
            if (blk.MemberCount == 1) return dSt;
            else return blk.FirstMember;
        }

        /// <summary>
        /// Refine the partitions by splitting incompatible states
        /// </summary>
        internal void RefinePartitions()
        {
            int generation = 0;
            while (worklist.Count > 0)
            {
                List<DFSA.DState> predSet = null;
                //
                // We are going to split all blocks with respect to a
                // particular (Block, Symbol) pair. 
                // Fetch the pair components.
                //
                // In order to avoid having to reset the predCount ints
                // all the time we keep a generation count to indicate to
                // which pass through the loop the counts apply.
                //
                PartitionBlock blk = worklist.Peek();

                int sym = blk.SymbolsLeft - 1;
                if (sym < 0)
                {
                    worklist.Pop();           // Remove block from the worklist.
                    continue;                 // Go around again to get new block.
                }
                generation++;
                predSet = new List<DFSA.DState>();
                //
                // Form a set of all those states that have a
                // next-state in "blk" on symbol "sym".
                // Note that despite the nested loops the 
                // complexity is only N == number of states.
                //
                foreach (DFSA.DState dSt in blk.members)
                {
                    if (dSt.HasPredecessors())
                    {
                        var prds = dSt.GetPredecessors(sym);
                        if (prds != null && prds.Count > 0)
                            foreach (DFSA.DState pSt in prds)
                                // The same predecessor state might appear on the
                                // list for more than one state of the blk.members.
                                // States should only appear once in the predSet "set"
                                if (pSt.listOrd != generation) // This predecessor is not in the set already
                                {
                                    PartitionBlock predBlk = (pSt.block as PartitionBlock);
                                    pSt.listOrd = generation;
                                    if (predBlk.Generation != generation)
                                    {
                                        predBlk.Generation = generation;
                                        predBlk.PredCount = 0;
                                    }
                                    predSet.Add(pSt);       // Add the predecessor to the set
                                    predBlk.PredCount++;
                                }
                    }
                }
                blk.SymbolsLeft--;                         // "Remove" (blk,sym) from the pair list.
                //
                // Now, if the predecessor set is not empty,
                // we split all blocks with respect to (blk,sym)
                //
                if (predSet.Count != 0)
                {
                    var splits = new List<PartitionBlock>();
                    foreach (DFSA.DState lSt in predSet)
                    {
                        PartitionBlock tBlk = null;
                        PartitionBlock lBlk = (lSt.block as PartitionBlock);
                        if (lBlk.PredCount != lBlk.MemberCount) // Need to split this block
                        {
                            tBlk = lBlk.twinBlk;
                            if (tBlk == null)
                            {
                                tBlk = MkNewBlock();
                                splits.Add(tBlk);
                                lBlk.twinBlk = tBlk;
                                tBlk.twinBlk = lBlk;
                            }
                            lBlk.MoveMember(lSt, tBlk);
                            lSt.block = tBlk;
                        }
                    }
                    foreach (PartitionBlock tBlk in splits)
                    {
                        PartitionBlock lBlk = tBlk.twinBlk;
                        PartitionBlock push = null;
                        if (lBlk.SymbolsLeft == 0)    // lBlk is not currently on the list so push smaller block
                        {
                            if (lBlk.MemberCount < tBlk.MemberCount)
                                push = lBlk;
                            else
                                push = tBlk;
                        }
                        else                 // lBlk is on the list with Sym > 0 
                        {
                            push = tBlk;
                            if (lBlk.MemberCount < tBlk.MemberCount)
                            {
                                // tBlk has the larger cardinality, so give it the smaller Sym value
                                tBlk.SymbolsLeft = lBlk.SymbolsLeft;
                                lBlk.SymbolsLeft = dfsa.MaxSym;
                            }
                        }
                        worklist.Push(push);
                        lBlk.twinBlk = null;
                        tBlk.twinBlk = null;
                    }
                }
            } // end of while loop...
        }
    }
}
