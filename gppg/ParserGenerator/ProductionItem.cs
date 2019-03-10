// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)



using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace QUT.GPGen
{
    internal class ProductionItem
    {
        internal Production production { get; private set; }
        internal int pos { get; private set; }
        internal bool expanded;
        internal SetCollection<Terminal> LookAhead;


        internal ProductionItem(Production production, int pos)
        {
            this.production = production;
            this.pos = pos;
        }


        public override bool Equals(object obj)
        {
            ProductionItem item = (ProductionItem)obj;
            return item.pos == pos && item.production == production;
        }

        public override int GetHashCode()
        {
            return production.GetHashCode() * 7 + pos;
        }


        internal static bool SameProductions(IEnumerable<ProductionItem> list1, IEnumerable<ProductionItem> list2)
        {
            if (list1.Count() != list2.Count())
                return false;

            return (!list1.Except(list2).Any() || !list2.Except(list1).Any());
        }


        internal bool IsReduction()
        {
            return pos == production.rhs.Count;
        }


        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat($"{production.num} {production.lhs}: ");


            for (int i = 0; i < production.rhs.Count; i++)
            {
                if (i == pos)
                    builder.Append(". ");
                builder.AppendFormat("{0} ", production.rhs[i]);
            }

            if (pos == production.rhs.Count)
                builder.Append(".");

            if (LookAhead != null)
            {
                builder.AppendLine();
                builder.AppendFormat("\t-lookahead: {0}", ListUtilities.GetStringFromList(LookAhead, ", ", 16));
            }

            return builder.ToString();
        }
    }
}