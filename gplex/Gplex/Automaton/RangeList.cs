// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;
using System.Collections.Generic;
using System.Linq;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// Represents a set of characters as a
    /// list of character range objects.
    /// </summary>
    internal class RangeList
    {
        // Notes:
        // This is a sparse representation of the character set. The
        // operations that are supported in primitive code are AND,
        // the inversion of the underlying List<CharRange>, and
        // value equality EQU. This is functionally complete, over
        // the Boolean operations. The set difference SUB is packaged
        // as AND (inverse of rh operand).

        /// <summary>
        /// Asserts that the list has been canonicalized, that is
        /// (1) the ranges are in sorted order of minChr
        /// (2) there is no overlap of ranges
        /// (3) contiguous ranges have been merged
        /// (3) invert is false.
        /// The set operations AND, SUB, EQU rely on this property!
        /// </summary>
        private bool isCanonical = true;
        private bool isAgnostic = false;
        private bool invert;

        private IList<CharRange> ranges;
        internal IList<CharRange> Ranges { get { return ranges; } }

        /// <summary>
        /// Construct a new RangeList with the given
        /// list of ranges.
        /// </summary>
        /// <param name="ranges">the list of ranges</param>
        /// <param name="invert">if true, means the inverse of the list</param>
        internal RangeList(IList<CharRange> ranges, bool invert)
        {
            this.invert = invert;
            this.ranges = ranges;
        }

        /// <summary>
        /// Construct an empty list, initialized with
        /// the invert flag specified.
        /// </summary>
        /// <param name="invert"></param>
        internal RangeList(bool invert)
        {
            this.invert = invert;
            ranges = new List<CharRange>();
        }

        internal bool IsEmpty { get { return ranges.Count == 0; } }
        internal bool IsInverted { get { return invert; } }

        internal void Add(CharRange rng)
        {
            ranges.Add(rng);  // AddToRange
            isCanonical = !invert && ranges.Count == 1;
        }

        /// <summary>
        /// Return a new RangeList that is the intersection of
        /// "this" and rhOp.  Neither operand is mutated.
        /// </summary>
        /// <param name="rhOp"></param>
        /// <returns></returns>
        internal RangeList AND(RangeList rhOp)
        {
            if (!isCanonical || !rhOp.isCanonical)
                throw new GplexInternalException("RangeList non canonicalized");
            if (this.ranges.Count == 0 || rhOp.ranges.Count == 0)
                return new RangeList(false); // return empty RangeList

            int thisIx;
            int rhOpIx = 0;
            int thisNm = this.ranges.Count;
            int rhOpNm = rhOp.ranges.Count;
            List<CharRange> newList = new List<CharRange>();
            RangeList result = new RangeList(newList, false);
            CharRange rhOpElem = rhOp.ranges[rhOpIx++];
            for (thisIx = 0; thisIx < thisNm; thisIx++)
            {
                CharRange thisElem = this.ranges[thisIx];
                // Attempt to find an overlapping element.
                // If necessary fetch new elements from rhOp
                // until maxChr of the new element is greater
                // than minChr of the current thisElem.
                while (rhOpElem.maxChr < thisElem.minChr)
                    if (rhOpIx < rhOpNm)
                        rhOpElem = rhOp.ranges[rhOpIx++];
                    else
                        return result;
                // It is possible that the rhOpElem is entirely beyond thisElem
                // It is also possible that rhOpElem and several following 
                // elements are all overlapping with thisElem.
                while (rhOpElem.minChr <= thisElem.maxChr)
                {
                    // process overlap
                    newList.Add(new CharRange(
                        (thisElem.minChr < rhOpElem.minChr ? rhOpElem.minChr : thisElem.minChr),
                        (thisElem.maxChr < rhOpElem.maxChr ? thisElem.maxChr : rhOpElem.maxChr)));
                    // If rhOpElem extends beyond thisElem.maxChr it is possible that
                    // it will overlap with the next thisElem, so do not advance rhOpIx.
                    if (rhOpElem.maxChr > thisElem.maxChr)
                        break;
                    else if (rhOpIx == rhOpNm)
                        return result;
                    else
                        rhOpElem = rhOp.ranges[rhOpIx++];
                }
            }
            return result;
        }

        /// <summary>
        /// Return a list of char ranges that represents
        /// the inverse of the set represented by "this".
        /// The RangeList must be sorted but not necessarily
        /// completely canonicalized.
        /// </summary>
        /// <returns></returns>
        internal IList<CharRange> InvertedList()
        {
            int index = 0;
            var result = new List<CharRange>();
            foreach (CharRange range in this.ranges)
            {
                if (range.minChr > index)
                    result.Add(new CharRange(index, (range.minChr - 1)));
                index = range.maxChr + 1;
            }
            if (index < CharRange.SymCard)
                result.Add(new CharRange(index, (CharRange.SymCard - 1)));
            return result;
        }

        /// <summary>
        /// Return the set difference of "this" and rhOp
        /// </summary>
        /// <param name="rhOp"></param>
        /// <returns></returns>
        internal RangeList SUB(RangeList rhOp)
        {
            if (!this.isCanonical || !rhOp.isCanonical)
                throw new GplexInternalException("RangeList not canonicalized");
            if (this.ranges.Count == 0)
                return new RangeList(false);
            else if (rhOp.ranges.Count == 0)
                return this;
            return this.AND(new RangeList(rhOp.InvertedList(), false));
        }

        /// <summary>
        /// Check value equality for "this" and rhOp.
        /// </summary>
        /// <param name="rhOp"></param>
        /// <returns></returns>
        internal bool EQU(RangeList rhOp)
        {
            if (!isCanonical || !rhOp.isCanonical)
                throw new GplexInternalException("RangeList not canonicalized");
            if (this == rhOp)
                return true;
            else if (this.ranges.Count != rhOp.ranges.Count)
                return false;
            else
            {
                for (int i = 0; i < this.ranges.Count; i++)
                    if (rhOp.ranges[i].CompareTo(ranges[i]) != 0)
                        return false;
                return true;
            }
        }

        /// <summary>
        /// Canonicalize the set. This may mutate
        /// both this.ranges and the invert flag.
        /// </summary>
        internal void Canonicalize()
        {
            if (this.isCanonical)
                return;
            if (!invert && this.ranges.Count <= 1)
            {
                this.isCanonical = true;
                return; // Empty, singleton and upper/lower pair RangeLists are trivially canonical
            }
            // Process non-empty lists.
            int listIx = 0;
            this.ranges = this.ranges.OrderBy(x => x).ToList();
            var newList = new List<CharRange>();
            CharRange currentRange = ranges[listIx++];
            while (listIx < ranges.Count)
            {
                CharRange nextRange = ranges[listIx++];
                if (nextRange.minChr > currentRange.maxChr + 1) // Merge contiguous ranges
                {
                    newList.Add(currentRange);
                    currentRange = nextRange;
                }
                else if
                    (nextRange.minChr <= (currentRange.maxChr + 1) &&
                     nextRange.maxChr >= currentRange.maxChr)
                {
                    currentRange = new CharRange(currentRange.minChr, nextRange.maxChr);
                }
                // Else skip ...
            }
            newList.Add(currentRange);
            this.ranges = newList;
            if (this.invert)
            {
                this.invert = false;
                this.ranges = this.InvertedList();
            }
            isCanonical = true;
        }

        /// <summary>
        /// Returns a new RangeList which is case-agnostic.
        /// The returned list will, in general, be seriously non-canonical.
        /// </summary>
        /// <returns>New case-insensitive list</returns>
        internal RangeList MakeCaseAgnosticList()
        {
            if (isAgnostic) return this; // Function is idempotent. Do not repeat.
            //
            // Do not canonicalize! We need to make the list case-
            // agnostic *before* we process the set inversion.
            //
            var agnosticList = new List<CharRange>();
            foreach (CharRange range in this.ranges)
            {
                for (int ch = range.minChr; ch <= range.maxChr; ch++)
                {
                    if (ch < char.MaxValue)
                    {
                        char c = (char)ch;
                        char lo = char.ToLower(c);
                        char hi = char.ToUpper(c);
                        if (lo == hi)
                            agnosticList.Add(new CharRange(c));
                        else
                        {
                            // There is a scary possibility with some 8-bit character
                            // sets that some characters may have case-pairs that are
                            // outside the character-set range limits. Must use guard!
                            //
                            if (lo < CharRange.SymCard)
                                agnosticList.Add(new CharRange(lo));
                            if (hi < CharRange.SymCard)
                                agnosticList.Add(new CharRange(hi));
                        }
                    }
                    else
                        agnosticList.Add(new CharRange(ch));
                }
            }
            var result = new RangeList(agnosticList, false);
            result.isCanonical = false;
            result.isAgnostic = true;
            result.invert = this.invert; // Result is inverted if input is.
            return result;
        }

#if RANGELIST_DIAGNOSTICS
        public override string ToString() {
            StringBuilder rslt = new StringBuilder();
            rslt.Append('[');
            if (invert)
                rslt.Append('^');
            if (!isCanonical)
                Canonicalize();
            foreach (CharRange range in ranges)
            {
                if (range.minChr == range.maxChr)
                    rslt.Append(CharacterUtilities.MapForCharSet(range.minChr));
                else
                {
                    rslt.Append(CharacterUtilities.MapForCharSet(range.minChr));
                    rslt.Append('-');
                    rslt.Append(CharacterUtilities.MapForCharSet(range.maxChr));
                }
            }
            rslt.Append(']');
            return rslt.ToString();
        }
#endif
    }
}
