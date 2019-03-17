// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;
using System.Globalization;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// Represents a contiguous range of characters
    /// between a given minimum and maximum values.
    /// </summary>
    internal class CharRange : IComparable<CharRange>
    {
        private static int symCard;
        internal static int SymCard { get { return symCard; } }
        internal static void Init(int num) { symCard = num; }
        internal static CharRange AllChars { get { return new CharRange(0, (symCard - 1)); } }

        internal int minChr;
        internal int maxChr;

        // internal CharRange(char min, char max) { minChr = (int)min; maxChr = (int)max; }
        internal CharRange(int min, int max) { minChr = min; maxChr = max; }

        internal CharRange(char chr) { minChr = maxChr = (int)chr; }
        internal CharRange(int chr) { minChr = maxChr = chr; }

        public override string ToString()
        {
            if (minChr == maxChr)
                return String.Format(CultureInfo.InvariantCulture, "singleton char {0}", CharacterUtilities.Map(minChr));
            else
                return String.Format(CultureInfo.InvariantCulture, "char range {0} .. {1}, {2} chars",
                    CharacterUtilities.Map(minChr), CharacterUtilities.Map(maxChr), maxChr - minChr + 1);
        }

        public int CompareTo(CharRange rhOp)
        {
            if (minChr < rhOp.minChr)
                return -1;
            else if (minChr > rhOp.minChr)
                return +1;
            else if (maxChr > rhOp.maxChr)
                // When two ranges start at the same minChr
                // we want the longer range to come first.
                return -1;
            else if (maxChr < rhOp.maxChr)
                return +1;
            else
                return 0;
        }
    }
}
