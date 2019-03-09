// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex
{
    internal partial class TreeSet
    {
        // ===================================================
        /// <summary>
        /// This class defines nodes for use in a set
        /// ADT based on binary trees. In this application
        /// we do not need deletion.
        /// </summary>
        class Node
        {
            Node lKid;
            Node rKid;
            int value;

            internal Node(int value) { this.value = value; }

            //private bool IsLeaf { get { return lKid == null && rKid == null; } }

            /// <summary>
            /// Check if key is a member of the set
            /// </summary>
            /// <param name="key">The key to check</param>
            /// <returns></returns>
            internal bool Lookup(int key)
            {
                if (key == value) return true;
                if (key < value)
                    return lKid != null && lKid.Lookup(key);
                else // (key > value)
                    return rKid != null && rKid.Lookup(key);
            }

            /// <summary>
            /// Insert key, if not already present.
            /// </summary>
            /// <param name="key">Value to insert</param>
            internal void Insert(int key)
            {
                if (key == value) return; // key is already present
                if (key < value)
                {
                    if (lKid == null)
                        lKid = new Node(key);
                    else
                        lKid.Insert(key);
                }
                else
                {
                    if (rKid == null)
                        rKid = new Node(key);
                    else
                        rKid.Insert(key);
                }
            }

            /// <summary>
            /// Delete key from tree rooted at tree if the key
            /// is present in the tree.  This is a static method 
            /// with a ref param to handle the special case of 
            /// deletion of a leaf node.
            /// </summary>
            /// <param name="tree"></param>
            /// <param name="key"></param>
            internal static void Delete(ref Node tree, int key)
            {
                if (tree == null)
                    return;
                else if (!tree.RemovedOk(key))
                    tree = null;
            }

            private bool RemovedOk(int key)
            {
                if (value == key)
                {
                    Node replacement = null;
                    if (lKid != null)
                    {
                        replacement = lKid.Largest();
                        value = replacement.value;
                        Delete(ref lKid, value);
                        return true;
                    }
                    else if (rKid != null)
                    {
                        replacement = rKid.Smallest();
                        value = replacement.value;
                        Delete(ref rKid, value);
                        return true;
                    }
                    else
                        return false;
                }
                else if (key < value && lKid != null)
                {
                    if (!lKid.RemovedOk(key)) lKid = null;
                }
                else if (key > value && rKid != null)
                {
                    if (!rKid.RemovedOk(key)) rKid = null;
                }
                return true; // No such member!
            }

            private Node Largest()
            {
                if (rKid == null) return this; else return rKid.Largest();
            }

            private Node Smallest()
            {
                if (lKid == null) return this; else return lKid.Smallest();
            }
        }
    }
}
