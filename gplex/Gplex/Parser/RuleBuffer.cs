// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System.Collections.Generic;

namespace QUT.Gplex.Parser
{
    internal sealed class RuleBuffer
    {
        List<LexSpan> locs = new List<LexSpan>();
        int fRuleLine, lRuleLine;  // First line of rules, last line of rules.

        internal int FLine { get { return fRuleLine; } set { fRuleLine = value; } }
        internal int LLine { get { return lRuleLine; } set { lRuleLine = value; } }

        internal void AddSpan(LexSpan l) { locs.Add(l); }

        /// <summary>
        /// This method detects the presence of code *between* rules. Such code has
        /// no unambiguous meaning, and is skipped, with a warning message.
        /// </summary>
        /// <param name="aast"></param>
        internal void FinalizeCode(AAST aast)
        {
            for (int i = 0; i < locs.Count; i++)
            {
                LexSpan loc = locs[i];

                if (loc.startLine < FLine)
                    aast.AddCodeSpan(AAST.Destination.scanProlog, loc);
                else if (loc.startLine > LLine)
                    aast.AddCodeSpan(AAST.Destination.scanEpilog, loc);
                else // code is between rules
                    aast.hdlr.ListError(loc, 110);
            }
        }
    }
}
