// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using QUT.Gplib;

namespace QUT.Gplex
{
    /// <summary>
    /// Nodes to represent contiguous ranges with same
    /// mapping in the Class Map. "value" is the map class 
    /// ordinal for all indices in the range min..max.
    /// </summary>
    public class TreeNode
    {
        private int _min;
        private int _max;
        private int _value;
        private TreeNode _leftKid;
        private TreeNode _rightKid;

        internal TreeNode(int mn, int mx, int vl)
        {
            _min = mn; _max = mx; _value = vl;
        }

        /// <summary>
        /// Fetch the bounds of the range that contains "index"
        /// </summary>
        /// <param name="index">Index value to lookup</param>
        /// <param name="rMin">Low bound of range</param>
        /// <param name="rMax">High bound of range</param>
        internal void GetRange(int index, out int rMin, out int rMax)
        {
            if (index < _min)
                _leftKid.GetRange(index, out rMin, out rMax);
            else if (index > _max)
                _rightKid.GetRange(index, out rMin, out rMax);
            else
            {
                rMin = _min;
                rMax = _max;
            }
        }

        /// <summary>
        /// Lookup the class ordinal for the range that contains index.
        /// </summary>
        /// <param name="index">The index to check</param>
        /// <returns>The class ordinal of the range</returns>
        internal int Lookup(int index)
        {
            if (index < _min)
                return _leftKid.Lookup(index);
            if (index > _max)
                return _rightKid.Lookup(index);
            return _value;
        }

        /// <summary>
        /// Insert a new Node in the tree.
        /// </summary>
        /// <param name="node">Node to insert</param>
        internal void InsertNewNode(TreeNode node)
        {
            int key = node._min;
            if (key < this._min)
            {
                if (_leftKid == null)
                    _leftKid = node;
                else
                    _leftKid.InsertNewNode(node);
            }
            else if (key > this._max)
            {
                if (_rightKid == null)
                    _rightKid = node;
                else
                    _rightKid.InsertNewNode(node);
            }
            else throw new ToolInternalException("Invalid range overlap");
        }
    }
}
