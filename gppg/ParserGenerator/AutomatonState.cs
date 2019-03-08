// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)


using System.Collections.Generic;
using System.Text;


namespace QUT.GPGen
{
    internal class AutomatonState
    {
        private static int TotalStates;

        internal int num;

        internal List<ProductionItem> kernelItems = new List<ProductionItem>();
        internal List<ProductionItem> allItems = new List<ProductionItem>();
        internal IDictionary<Symbol, AutomatonState> Goto = new Dictionary<Symbol, AutomatonState>();
        internal SetCollection<Terminal> terminalTransitions = new SetCollection<Terminal>();
        internal IDictionary<NonTerminal, Transition> nonTerminalTransitions = new Dictionary<NonTerminal, Transition>();
        internal IDictionary<Terminal, ParserAction> parseTable = new Dictionary<Terminal, ParserAction>();

        internal IList<Symbol> shortestPrefix;
        internal IList<AutomatonState> statePath;
        internal IList<Conflict> conflicts;

        internal AutomatonState(Production production)
        {
            num = TotalStates++;
            AddKernel(production, 0);
        }


        internal AutomatonState(IList<ProductionItem> itemSet)
        {
            num = TotalStates++;
            kernelItems.AddRange(itemSet);
            allItems.AddRange(itemSet);
        }


        internal void AddClosure()
        {
            foreach (ProductionItem item in kernelItems)
                AddClosure(item);
        }


        private void AddClosure(ProductionItem item)
        {
            if (item.pos < item.production.rhs.Count)
            {
                Symbol rhs = item.production.rhs[item.pos];
                NonTerminal nonTerm = rhs as NonTerminal;
                if (nonTerm != null)
                {
                    foreach (Production p in nonTerm.productions)
                        AddNonKernel(p);
                }
            }
        }


        private void AddKernel(Production production, int pos)
        {
            var item = new ProductionItem(production, pos);
            kernelItems.Add(item);
            allItems.Add(item);
        }


        private void AddNonKernel(Production production)
        {
            var item = new ProductionItem(production, 0);

            if (!allItems.Contains(item))
            {
                allItems.Add(item);
                AddClosure(item);
            }
        }


        internal void AddGoto(Symbol s, AutomatonState next)
        {
            this.Goto[s] = next;
            Terminal term = s as Terminal;

            if (term != null)
                terminalTransitions.Add(term);
            else
            {
                NonTerminal nonTerm = (NonTerminal)s;
                nonTerminalTransitions.Add(nonTerm, new Transition(nonTerm, next));
            }
        }

        internal string ItemDisplay()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("State {0}", num);
            foreach (ProductionItem item in kernelItems)
            {
                builder.AppendLine();
                builder.AppendFormat("    {0}", item);
            }
            return builder.ToString();
        }


        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendFormat("State {0}", num);
            builder.AppendLine();
            builder.AppendLine();

            foreach (var item in kernelItems)
            {
                builder.AppendFormat("    {0}", item);
                builder.AppendLine();
            }

            builder.AppendLine();

            foreach (var a in parseTable)
            {
                builder.AppendFormat("    {0,-14} {1}", a.Key, a.Value);
                builder.AppendLine();
            }

            builder.AppendLine();

            foreach (var n in nonTerminalTransitions)
            {
                builder.AppendFormat("    {0,-14} go to state {1}", n.Key, Goto[n.Key].num);
                builder.AppendLine();
            }

            builder.AppendLine();

            return builder.ToString();
        }

        internal void Link(Conflict conflict)
        {
            conflicts = conflicts ?? new List<Conflict>();
            conflicts.Add(conflict);
        }
    }
}