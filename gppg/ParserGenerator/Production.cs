// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)


using System.Text;
using System.Collections.Generic;
using QUT.Gplib;

namespace QUT.GPGen
{
    internal class Production : IProduction
    {
        internal int num;
        internal NonTerminal lhs;
        internal IList<ISymbol> rhs = new List<ISymbol>();
        internal SemanticAction semanticAction;
        internal Precedence prec;
        internal Parser.LexSpan precSpan;

        internal Production(NonTerminal lhs)
        {
            this.lhs = lhs;
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
    }
}