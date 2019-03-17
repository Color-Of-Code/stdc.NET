// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;
using System.Collections;
using System.Collections.Generic;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// Represents a character set literal as a
    /// list of character ranges.  A direct mapping
    /// of a LEX character set "[...]".
    /// The field equivClasses holds the list of
    /// ordinals of the partition elements that
    /// cover the characters of the literal.
    /// </summary>
    internal class RangeLiteral
    {
        internal string name;
        static RangeLiteral rAnchor;

        /// <summary>
        /// The last partition that this literal has refined
        /// </summary>
        internal Partition lastPart;

        internal RangeList list;
        internal IList<int> equivClasses = new List<int>();

        internal RangeLiteral(bool invert) { list = new RangeList(invert); }
        internal RangeLiteral(int ch)
        {
            list = new RangeList(false);
            list.Add(new CharRange(ch, ch)); // AddToRange
        }
        private RangeLiteral(int lo, int hi)
        {
            list = new RangeList(false);
            list.Add(new CharRange(lo, lo));
            list.Add(new CharRange(hi, hi));
            list.Canonicalize();
        }

        /// <summary>
        /// If ch represents a charater with different upper and lower
        /// case codes, return a RangeLiteral representing the pair.
        /// Otherwise return a singleton RangeLiteral.
        /// </summary>
        /// <param name="ch">the code point to test</param>
        /// <returns>a pair or a singleton RangeLiteral</returns>
        internal static RangeLiteral NewCaseAgnosticPair(int ch)
        {
            if (ch < Char.MaxValue)
            {
                char c = (char)ch;
                char lo = char.ToLower(c);
                char hi = char.ToUpper(c);
                if (lo != hi)
                    return new RangeLiteral(lo, hi);
            }
            return new RangeLiteral(ch);
        }

        public override string ToString() { return name; }

        internal void MakeCaseAgnosticList()
        {
            list = list.MakeCaseAgnosticList();
            list.Canonicalize();
        }

        internal BitArray GetBitArray(int maxSym, bool pack)
        {
            var cls = new BitArray(maxSym);
            if (pack)
            {
                foreach (int ord in equivClasses)
                    cls[ord] = true;
            }
            else
            {
                foreach (CharRange rng in list.Ranges)
                    for (int i = rng.minChr; i <= rng.maxChr; i++)
                        cls[i] = true;
                if (list.IsInverted)
                    cls = cls.Not();
            }
            return cls;
        }

        /// <summary>
        /// The RangeLiteral for all line-end characters.
        /// For ASCII case just [\n\r], for 
        /// unicode [\r\n\u0085\u2028\u2029]
        /// </summary>
        internal static RangeLiteral RightAnchors
        {
            get
            {
                if (rAnchor == null)
                {
                    rAnchor = new RangeLiteral(false);
                    rAnchor.list.Add(new CharRange('\r'));
                    rAnchor.list.Add(new CharRange('\n'));
                    if (CharRange.SymCard > 256)
                    {
                        rAnchor.list.Add(new CharRange('\x85'));
                        rAnchor.list.Add(new CharRange('\u2028', '\u2029'));
                    }
                }
                return rAnchor;
            }
        }
    }
}
