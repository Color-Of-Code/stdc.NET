// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)


using System.Collections.Generic;
using System.Text;
using QUT.Gplib;

namespace QUT.GPGen
{
    internal class AutomatonState: IState
    {
        private static IdGenerator _ids = new IdGenerator();

        public int num { get; private set; }

        public List<IProductionRule> kernelItems { get; private set; }
        internal List<ProductionItem> AllProductionItems = new List<ProductionItem>();
        internal IDictionary<ISymbol, AutomatonState> Goto = new Dictionary<ISymbol, AutomatonState>();
        internal HashSet<Terminal> terminalTransitions = new HashSet<Terminal>();
        internal IDictionary<NonTerminal, Transition> nonTerminalTransitions = new Dictionary<NonTerminal, Transition>();
        internal IDictionary<Terminal, IParserAction> parseTable = new Dictionary<Terminal, IParserAction>();

        internal IList<ISymbol> shortestPrefix;
        internal IList<AutomatonState> statePath;
        internal IList<Conflict> conflicts;

        private AutomatonState()
        {
            num = _ids.Next();
            kernelItems = new List<IProductionRule>();
        }

        internal AutomatonState(Production production)
            : this()
        {
            AddKernel(production, 0);
        }


        internal AutomatonState(IList<ProductionItem> itemSet)
            : this()
        {
            kernelItems.AddRange(itemSet);
            AllProductionItems.AddRange(itemSet);
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
                ISymbol rhs = item.production.rhs[item.pos];
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
            AllProductionItems.Add(item);
        }


        private void AddNonKernel(Production production)
        {
            var item = new ProductionItem(production, 0);

            if (!AllProductionItems.Contains(item))
            {
                AllProductionItems.Add(item);
                AddClosure(item);
            }
        }


        internal void AddGoto(ISymbol s, AutomatonState next)
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

        internal void AddShiftActions()
        {
            foreach (var t in terminalTransitions)
                parseTable[t] = new Shift(Goto[t]);
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