// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System.Collections.Generic;

namespace QUT.Gplex.Automaton
{
    internal class PartitionBlock
    {
        internal PartitionBlock twinBlk;       //  During a split, the two fragments reference each other.
        internal LinkedList<DFSA.DState> members;

        //  Number of symbols left on the "pair-list" for this block.
        public int SymbolsLeft { get; set; }
        //  The current splitting generation.
        public int Generation { get; set; }
        //  Number of predecessors from the current generation.
        public int PredCount { get; set; }
        public int MemberCount { get { return members.Count; } }
        public DFSA.DState FirstMember { get { return members.First.Value; } }

        /// <summary>
        /// Add the given node to the linked list
        /// </summary>
        /// <param name="node"></param>
        private void AddNode(LinkedListNode<DFSA.DState> node)
        {
            this.members.AddLast(node);
        }

        /// <summary>
        /// Add a new node to the list, with value given by the dSt
        /// </summary>
        /// <param name="dSt"></param>
        internal void AddState(DFSA.DState dSt)
        {
            var node = new LinkedListNode<DFSA.DState>(dSt);
            dSt.listNode = node;
            this.members.AddLast(node);
        }

        /// <summary>
        /// Move the node with value dSt from this partition to blk.
        /// </summary>
        /// <param name="dSt">value to be moved</param>
        /// <param name="blk">destination partition</param>
        internal void MoveMember(DFSA.DState dSt, PartitionBlock blk)
        {
            // Assert: dSt must belong to LinkedList this.members
            var node = dSt.listNode;
            this.members.Remove(node);
            this.PredCount--;
            blk.AddNode(node);
        }

        internal PartitionBlock(int symbolCardinality)
        {
            SymbolsLeft = symbolCardinality;            // Default cardinality of symbol alphabet.
            members = new LinkedList<DFSA.DState>();
        }
    }
}
