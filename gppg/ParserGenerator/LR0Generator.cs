// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using QUT.Gplib;

namespace QUT.GPGen
{
    internal class LR0Generator
    {
        protected IList<AutomatonState> states = new List<AutomatonState>();
        protected Grammar grammar;
        private IDictionary<ISymbol, IList<AutomatonState>> accessedBy = new Dictionary<ISymbol, IList<AutomatonState>>();


        internal LR0Generator(Grammar grammar)
        {
            this.grammar = grammar;
        }


        internal IList<AutomatonState> BuildStates()
        {
            // create state for root production and expand recursively
            ExpandState(grammar.rootProduction.lhs, new AutomatonState(grammar.rootProduction));

            return states;
        }

        private void ExpandState(ISymbol sym, AutomatonState newState)
        {
            //newState.accessedBy = sym;
            states.Add(newState);

            if (!accessedBy.ContainsKey(sym))
                accessedBy[sym] = new List<AutomatonState>();
            accessedBy[sym].Add(newState);

            newState.AddClosure();
            ComputeGoto(newState);
        }

        private void ComputeGoto(AutomatonState state)
        {
            foreach (var item in state.AllProductionItems)
                if (!item.expanded && !item.IsReduction())
                {
                    item.expanded = true;
                    ISymbol s1 = item.production.rhs[item.pos];

                    // Create itemset for new state ...
                    var itemSet = new List<ProductionItem>();
                    itemSet.Add(new ProductionItem(item.production, item.pos + 1));

                    foreach (ProductionItem item2 in state.AllProductionItems
                        .Where(x => !x.expanded && !x.IsReduction()))
                    {
                        ISymbol s2 = item2.production.rhs[item2.pos];
                        if (s1 == s2)
                        {
                            item2.expanded = true;
                            itemSet.Add(new ProductionItem(item2.production, item2.pos + 1));
                        }
                    }

                    AutomatonState existingState = FindExistingState(s1, itemSet);

                    if (existingState == null)
                    {
                        AutomatonState newState = new AutomatonState(itemSet);
                        state.AddGoto(s1, newState);
                        ExpandState(s1, newState);
                    }
                    else
                        state.AddGoto(s1, existingState);
                }
        }

        private AutomatonState FindExistingState(ISymbol sym, List<ProductionItem> itemSet)
        {
            if (accessedBy.ContainsKey(sym))
                return accessedBy[sym]
                    .FirstOrDefault(x => ProductionItem.SameProductions(x.KernelItems, itemSet));
            return null;
        }

        internal void BuildParseTable()
        {
            foreach (var state in states)
            {
                // Add shift actions ...
                state.AddShiftActions();

                // Add reduce actions ...
                var reductions = state.AllProductionItems.Where(x => x.IsReduction());
                foreach (var item in reductions)
                {
                    // Accept on everything
                    if (item.production == grammar.rootProduction)
                        foreach (var t in grammar.terminals.Values)
                            state.parseTable[t] = new Reduce(item);

                    foreach (var t in item.LookAhead)
                        BuildParseTable(state, item, t);
                }
            }
        }

        private void BuildParseTable(AutomatonState state, ProductionItem item, Terminal t)
        {
            // possible conflict with existing action
            if (state.parseTable.ContainsKey(t))
            {
                Reduce reduceAction;
                var other = state.parseTable[t];
                var iProd = item.production;
                if ((reduceAction = other as Reduce) != null)
                {
                    Production oProd = reduceAction.item.production;

                    // Choose in favour of production listed first in the grammar
                    if (oProd.num > iProd.num)
                        state.parseTable[t] = new Reduce(item);

                    string p1 = $" Reduce {oProd.num}:\t{oProd.ToString()}";
                    string p2 = $" Reduce {iProd.num}:\t{iProd.ToString()}";
                    int chsn = (oProd.num > iProd.num ? iProd.num : oProd.num);
                    grammar.conflicts.Add(new ReduceReduceConflict(t, p1, p2, chsn, state));
                    if (GPCG.Verbose)
                    {
                        Console.Error.WriteLine(
                            "Reduce/Reduce conflict in state {0} on symbol {1}",
                            state.Id,
                            t.ToString());
                        Console.Error.WriteLine(p1);
                        Console.Error.WriteLine(p2);
                    }
                    else
                        Console.Error.WriteLine("Reduce/Reduce conflict, state {0}: {1} vs {2} on {3}",
                                                                    state.Id, iProd.num, oProd.num, t);
                }
                else
                {
                    if (iProd.prec != null && t.prec != null)
                    {
                        if (iProd.prec.prec > t.prec.prec ||
                            (iProd.prec.prec == t.prec.prec &&
                                iProd.prec.type == PrecedenceType.left))
                        {
                            // resolve in favour of reduce (without error)
                            state.parseTable[t] = new Reduce(item);
                        }
                        else
                        {
                            // resolve in favour of shift (without error)
                        }
                    }
                    else
                    {
                        AutomatonState next = ((Shift)other).next;
                        string p1 = $" Shift \"{t}\":\tState-{state.Id} -> State-{next.Id}";
                        string p2 = $" Reduce {iProd.num}:\t{iProd.ToString()}";
                        grammar.conflicts.Add(new ShiftReduceConflict(t, p1, p2, state, next));
                        if (GPCG.Verbose)
                        {
                            Console.Error.WriteLine("Shift/Reduce conflict");
                            Console.Error.WriteLine(p1);
                            Console.Error.WriteLine(p2);
                        }
                        else
                            Console.Error.WriteLine("Shift/Reduce conflict, state {0} on {1}", state.Id, t);
                    }
                    // choose in favour of the shift
                }
            }
            else
                state.parseTable[t] = new Reduce(item);
        }
    }
}
