// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System.Collections.Generic;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    internal sealed class RuleBuffer
    {
        IList<ISpan> locs = new List<ISpan>();

        // First line of rules
        internal int FLine { get; set; }
        // Last line of rules.
        internal int LLine { get; set; }

        internal void AddSpan(LexSpan l) { locs.Add(l); }

        /// <summary>
        /// This method detects the presence of code *between* rules. Such code has
        /// no unambiguous meaning, and is skipped, with a warning message.
        /// </summary>
        /// <param name="aast"></param>
        internal void FinalizeCode(AAST aast)
        {
            foreach (ISpan loc in locs)
            {
                if (loc.StartLine < FLine)
                    aast.AddCodeSpan(Destination.scanProlog, loc);
                else if (loc.StartLine > LLine)
                    aast.AddCodeSpan(Destination.scanEpilog, loc);
                else // code is between rules
                    aast.hdlr.ListError(loc, 110);
            }
        }
    }
}
