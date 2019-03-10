// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using System;
using System.Linq;

namespace QUT.GPGen
{

    internal class Precedence
    {
        internal PrecedenceType type { get; private set; }
        internal int prec { get; private set; }

        internal Precedence(PrecedenceType type, int prec)
        {
            this.type = type;
            this.prec = prec;
        }

        internal static void Calculate(Production p)
        {
            // Precedence of a production is that of its rightmost terminal
            // unless explicitly labelled with %prec

            if (p.prec != null)
                return;

            var rightMostTerminal = p.rhs
                .Reverse()
                .FirstOrDefault(x => x is Terminal);
            
            if (rightMostTerminal != null)
            {
                p.prec = ((Terminal)rightMostTerminal).prec;
            }
        }
    }
}