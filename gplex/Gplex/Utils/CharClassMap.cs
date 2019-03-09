// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex
{
    /// <summary>
    /// This class maps from a character ordinal to a character class.
    /// A well-formed CharClassMap must map to a class for every 
    /// index in the complete character ordinal range.
    /// </summary>
    internal class CharClassMap
    {
        // ==========================================================
        /// <summary>
        /// Nodes to represent contiguous ranges with same
        /// mapping in the Class Map. "value" is the map class 
        /// ordinal for all indices in the range min..max.
        /// </summary>
        class TreeNode
        {
            int min;
            int max;
            int value;
            TreeNode lKid;
            TreeNode rKid;

            internal TreeNode(int mn, int mx, int vl)
            {
                min = mn; max = mx; value = vl;
            }

            /// <summary>
            /// Fetch the bounds of the range that contains "index"
            /// </summary>
            /// <param name="index">Index value to lookup</param>
            /// <param name="rMin">Low bound of range</param>
            /// <param name="rMax">High bound of range</param>
            internal void GetRange(int index, out int rMin, out int rMax)
            {
                if (index < min) lKid.GetRange(index, out rMin, out rMax);
                else if (index > max) rKid.GetRange(index, out rMin, out rMax);
                else
                {
                    rMin = min;
                    rMax = max;
                }
            }

            /// <summary>
            /// Lookup the class ordinal for the range that contains index.
            /// </summary>
            /// <param name="index">The index to check</param>
            /// <returns>The class ordinal of the range</returns>
            internal int Lookup(int index)
            {
                if (index < min) return lKid.Lookup(index);
                else if (index > max) return rKid.Lookup(index);
                else return value;
            }

            /// <summary>
            /// Insert a new Node in the tree.
            /// </summary>
            /// <param name="node">Node to insert</param>
            internal void InsertNewNode(TreeNode node)
            {
                int key = node.min;
                if (key < this.min)
                {
                    if (this.lKid == null)
                        this.lKid = node;
                    else
                        lKid.InsertNewNode(node);
                }
                else if (key > this.max)
                {
                    if (this.rKid == null)
                        this.rKid = node;
                    else
                        rKid.InsertNewNode(node);
                }
                else throw new Parser.GplexInternalException("Invalid range overlap");
            }
        }
        // ==========================================================

        int count;
        TreeNode root;

        internal int this[int theChar]
        {
            get
            {
                if (root == null) throw new Parser.GplexInternalException("Map not initialized");
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
        internal CharClassMap(Parser.Partition part)
        {

            foreach (Parser.PartitionElement pElem in part.elements)
            {
                foreach (Parser.CharRange range in pElem.list.Ranges)
                {
                    count++;
                    TreeNode node = new TreeNode(range.minChr, range.maxChr, pElem.ord);
                    if (root == null) root = node; else root.InsertNewNode(node);
                }
            }
        }

        //internal int Depth { get { return root.Depth; } }
        //internal int Count { get { return this.count; } }
    }
}
