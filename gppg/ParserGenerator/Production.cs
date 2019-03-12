// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using System.Text;
using System.Linq;
using System.Collections.Generic;
using QUT.Gplib;

namespace QUT.GPGen
{
    public class Production : IProduction
    {
        internal int num;
        public NonTerminal lhs { get; private set; }
        public IList<ISymbol> rhs { get; private set; }
        internal SemanticAction semanticAction;
        internal Precedence prec;
        internal ISpan precSpan;

        internal Production(NonTerminal lhs)
        {
            this.lhs = lhs;
            this.rhs = new List<ISymbol>();
        
            lhs.productions.Add(this);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0} -> ", lhs);
            if (rhs.Count == 0)
                builder.Append("/* empty */");
            else
                builder.Append(ListUtilities.GetStringFromList(rhs, ", ", builder.Length));
            return builder.ToString();
        }

                // terminates if all non terminal symbols terminate
        public bool Terminates()
        {
            return this.rhs
                .Where(x => x is NonTerminal)
                .All(x => (x as NonTerminal).IsTerminating);
        }


    }
}