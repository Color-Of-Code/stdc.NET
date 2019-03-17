// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System.Collections.Generic;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// This class represents a single partition in 
    /// a partition set. Each such element denotes
    /// a set of characters that belong to the same
    /// equivalence class with respect to the literals
    /// already processed.
    /// </summary>
    internal class PartitionElement
    {
        private static IdGenerator _ids = new IdGenerator();

        internal static void Reset()
        { _ids = new IdGenerator(); }

        internal static PartitionElement AllChars()
        {
            List<CharRange> singleton = new List<CharRange>();
            singleton.Add(CharRange.AllChars);
            return new PartitionElement(singleton, false);
        }


        internal int ord;
        internal RangeList list;

        /// <summary>
        /// List of literals that contain this partition element.
        /// </summary>
        internal IList<RangeLiteral> literals = new List<RangeLiteral>();

        internal PartitionElement(IList<CharRange> ranges, bool invert)
        {
            ord = _ids.Next();
            list = new RangeList(ranges, invert);
        }
    }
}
