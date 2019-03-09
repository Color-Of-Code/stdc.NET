// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex
{
    /// <summary>
    /// A Membership Set ADT implemented as a binary tree.
    /// Insert, Delete and Lookup implemented.
    /// </summary>
    internal partial class TreeSet
    {
        // ===================================================

        private Node root;

        internal bool this[int thisChar]
        {
            get { return root != null && root.Lookup(thisChar); }
            set
            {
                if (value == false)
                    Node.Delete(ref root, thisChar);
                else if (root == null)
                    root = new Node(thisChar);
                else
                    root.Insert(thisChar);
            }
        }
    }
}
