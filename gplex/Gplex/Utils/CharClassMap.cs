// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using QUT.Gplex.Parser;
using QUT.Gplib;

namespace QUT.Gplex
{
    /// <summary>
    /// This class maps from a character ordinal to a character class.
    /// A well-formed CharClassMap must map to a class for every 
    /// index in the complete character ordinal range.
    /// </summary>
    internal partial class CharClassMap
    {
        int count;
        TreeNode root;

        internal int this[int theChar]
        {
            get
            {
                if (root == null) throw new ToolInternalException("Map not initialized");
                return root.Lookup(theChar);
            }
        }

        internal void GetEnclosingRange(int theChar, out int min, out int max)
        {
            root.GetRange(theChar, out min, out max);
        }

        /// <summary>
        /// Create a well-formed (that is, *complete*) 
        /// map from a GPLEX.Parser.Partition.
        /// </summary>
        /// <param name="part">The Partition to use</param>
        internal CharClassMap(Partition part)
        {
            foreach (PartitionElement pElem in part.elements)
            {
                foreach (CharRange range in pElem.list.Ranges)
                {
                    count++;
                    TreeNode node = new TreeNode(range.minChr, range.maxChr, pElem.ord);
                    if (root == null) root = node; else root.InsertNewNode(node);
                }
            }
        }
    }
}
