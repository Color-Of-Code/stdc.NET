// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using QUT.GPGen.Parser;
using QUT.Gplib;

namespace QUT.GPGen
{
    internal class Grammar : IGrammar
    {
        internal const string DefaultValueTypeName = "ValueType";
        internal const int LineLength = 80;

        private int currentPrec;
        internal void BumpPrec() { currentPrec += 10; }
        internal int Prec { get { return currentPrec; } }

        internal IList<Production> productions = new List<Production>();
        internal LexSpan unionType;
        internal int NumActions;
        internal IList<LexSpan> prologCode = new List<LexSpan>();	// before first %%
        internal LexSpan epilogCode;    // after last %%
        internal NonTerminal startSymbol;
        internal Production rootProduction;
        internal IDictionary<string, NonTerminal> nonTerminals = new Dictionary<string, NonTerminal>();
        internal IDictionary<string, Terminal> terminals = new Dictionary<string, Terminal>();
        internal IDictionary<string, Terminal> aliasTerms = new Dictionary<string, Terminal>();
        internal IList<string> usingList = new List<string>();
        internal IList<Conflict> conflicts = new List<Conflict>();

        internal bool IsPartial;
        internal string OutFileName;
        internal string TokFileName;
        internal string DiagFileName;
        internal string InputFileIdent;
        internal string InputFilename;
        internal string Namespace;
        internal string Visibility = "public";
        internal string ParserName = "Parser";
        internal string TokenName = "Tokens";
        internal string ScanBaseName = "ScanBase";
        internal string ValueTypeName;
        internal string LocationTypeName = "LexLocation";
        internal string PartialMark { get { return (IsPartial ? " partial" : ""); } }
        internal LexSpan ValueTypeNameSpan;

        // Experimental features
        // readonly List<Terminal> emptyTerminalList;
        private ErrorHandler handler;

        internal bool HasNonTerminatingNonTerms
        {
            get;
            private set;
        }
        // end

        internal Grammar()
        {
            LookupTerminal(Token.ident, "error");
            LookupTerminal(Token.ident, "EOF");
            // emptyTerminalList = new List<Terminal>();
        }


        internal Terminal LookupTerminal(Token token, string name)
        {
            bool isIdent = (token == Token.ident);
            // Canonicalize escaped char-literals
            if (!isIdent)
                name = CharacterUtilities.Canonicalize(name, 1);
            // Check if already present in dictionary
            if (!terminals.ContainsKey(name)) // else insert ...
                terminals[name] = new Terminal(isIdent, name);

            return terminals[name];
        }

        internal Terminal LookupOrDefineTerminal(Token token, string name, string alias)
        {
            bool isIdent = (token == Token.ident);
            // Canonicalize escaped char-literals
            if (!isIdent)
                name = CharacterUtilities.Canonicalize(name, 1);
            // Check if already present in dictionary
            if (!terminals.ContainsKey(name)) // else insert ...
            {
                Terminal newTerm = new Terminal(isIdent, name, alias);
                terminals[name] = newTerm;
                if (alias != null)
                    aliasTerms[alias] = newTerm;
            }

            return terminals[name];
        }


        internal NonTerminal LookupNonTerminal(string name)
        {
            if (!nonTerminals.ContainsKey(name))
                nonTerminals[name] = new NonTerminal(name);

            return nonTerminals[name];
        }

        internal void AddProduction(Production production)
        {
            productions.Add(production);
            production.num = productions.Count;
        }

        internal void CreateSpecialProduction(NonTerminal root)
        {
            rootProduction = new Production(LookupNonTerminal("$accept"));
            AddProduction(rootProduction);
            rootProduction.rhs.Add(root);
            rootProduction.rhs.Add(LookupTerminal(Token.ident, "EOF"));
        }

        void MarkReachable()
        {
            var work = new Stack<NonTerminal>();
            rootProduction.lhs.reached = true; // by definition.
            work.Push(startSymbol);
            startSymbol.reached = true;
            while (work.Any())
            {
                NonTerminal nonT = work.Pop();
                foreach (Production prod in nonT.productions)
                {
                    foreach (ISymbol smbl in prod.rhs)
                    {
                        NonTerminal rhNt = smbl as NonTerminal;
                        if (rhNt != null && !rhNt.reached)
                        {
                            rhNt.reached = true;
                            work.Push(rhNt);
                        }
                    }
                }
            }
        }


        // =============================================================================
        #region Terminating Computation
        // =============================================================================

        const int finishMark = int.MaxValue;

        /// <summary>
        /// This is the method that computes the shortest terminal
        /// string sequence for each NonTerminal symbol.  The immediate
        /// guide is to find those NT that are non-terminating.
        /// </summary>
        void MarkTerminating()
        {
            bool changed = false;
            int nonTerminatingCount = 0;
            // This uses a naive algorithm that iterates until
            // an iteration completes without changing anything.
            do
            {
                changed = false;
                nonTerminatingCount = 0;
                foreach (var kvp in this.nonTerminals)
                {
                    NonTerminal nonTerm = kvp.Value;
                    if (!nonTerm.IsTerminating())
                    {
                        if (nonTerm.productions.Any(x => x.Terminates()))
                        {
                            nonTerm.SetTerminating();
                            changed = true;
                        }
                        if (!nonTerm.IsTerminating())
                            nonTerminatingCount++;
                    }
                }
            } while (changed);
            //
            // Now produce some helpful diagnostics.
            // We wish to find single NonTerminals that, if made
            // terminating will fix up many, even all of the
            // non-terminating NonTerminals that have been found.
            //
            if (nonTerminatingCount > 0)
            {
                var ntDependencies = BuildDependencyGraph();
                HasNonTerminatingNonTerms = true;
                handler.AddError(
                    String.Format(CultureInfo.InvariantCulture, "There are {0} non-terminating NonTerminal Symbols{1} {{{2}}}",
                        nonTerminatingCount,
                        System.Environment.NewLine,
                        ListUtilities.GetStringFromList(ntDependencies)), null);

                FindNonTerminatingSCC(ntDependencies); // Do some diagnosis
            }
        }

        //
        // NonTerminals that are non-terminating are usually so because
        // they depend on other NonTerms that are themselves non-terminating.
        // We first construct a graph modelling these dependencies, and then
        // find strongly connected regions in the dependency graph.
        //
        private void FindNonTerminatingSCC(IList<NonTerminal> ntDependencies)
        {
            int count = 0;
            // ntStack is the working stack used to find Strongly Connected 
            // Components, hereafter referred to as SCC.
            var ntStack = new Stack<NonTerminal>();

            // candidates is the list of states that *might* be to blame.
            // These are two groups: leaves of the dependency graph, and
            // NonTerminals that fix up a complete SCC
            var candidates = new List<NonTerminal>();
            foreach (NonTerminal nt in ntDependencies)
            {
                if (nt.dependsOnList.Count == 0)
                    candidates.Add(nt);
                else if (nt.depth != finishMark)
                    Walk(nt, ntStack, candidates, ref count);
            }
            foreach (NonTerminal candidate in candidates)
                LeafExperiment(candidate, ntDependencies);
        }

        private void Walk(NonTerminal node, Stack<NonTerminal> stack, List<NonTerminal> fixes, ref int count)
        {
            count++;
            stack.Push(node);
            node.depth = count;
            foreach (NonTerminal next in node.dependsOnList)
            {
                if (next.depth == 0)
                    Walk(next, stack, fixes, ref count);
                if (next.depth < count)
                    node.depth = next.depth;
            }
            if (node.depth == count) // traversal leaving strongly connected component
            {
                // This algorithm is folklore. I have been using it since
                // at least early 1980s in the Gardens Point compilers.
                // I don't even remember where I learned it ... (kjg).
                //
                NonTerminal popped = stack.Pop();
                popped.depth = finishMark;
                if (popped != node)
                {
                    var SCC = new List<NonTerminal>();
                    SCC.Add(popped);
                    do
                    {
                        popped = stack.Pop();
                        popped.depth = finishMark;
                        SCC.Add(popped);
                    }
                    while (popped != node);
                    handler.AddWarning(String.Format(CultureInfo.InvariantCulture,
                        "The following {2} symbols form a non-terminating cycle {0}{{{1}}}",
                        System.Environment.NewLine,
                        ListUtilities.GetStringFromList(SCC),
                        SCC.Count), null);
                    //
                    // Check if termination of any single NonTerminal
                    // would eliminate the whole cycle of dependency.
                    //
                    SccExperiment(SCC, fixes);
                }
            }
            count--;
        }

        private IList<NonTerminal> BuildDependencyGraph()
        {
            var rslt = new List<NonTerminal>();
            foreach (var kvp in this.nonTerminals)
            {
                NonTerminal nonTerm = kvp.Value;
                NonTerminal dependency = null;
                if (!nonTerm.IsTerminating())
                {
                    rslt.Add(nonTerm);
                    nonTerm.dependsOnList = new List<NonTerminal>();
                    foreach (Production prod in nonTerm.productions)
                        foreach (ISymbol symbol in prod.rhs)
                        {
                            dependency = symbol as NonTerminal;
                            if (dependency != null &&
                                dependency != nonTerm &&
                                !dependency.IsTerminating() &&
                                !nonTerm.dependsOnList.Contains(dependency))
                            {
                                nonTerm.depth = 0;
                                nonTerm.dependsOnList.Add(dependency);
                            }
                        }

                }
            }
            return rslt;
        }

        private static void SccExperiment(IList<NonTerminal> component, IList<NonTerminal> fixes)
        {
            foreach (NonTerminal probe in component)
            {
                // Test what happens with probe nullable ...
                probe.SetTerminating();
                SccPropagate(probe, component, fixes);
                // Then reset the values of all components
                foreach (NonTerminal element in component)
                    element.SetTerminating(false);
            }
        }

        private static void SccPropagate(NonTerminal root, IList<NonTerminal> thisTestConfig, IList<NonTerminal> fixes)
        {
            int count = 0;
            bool changed = false;
            do
            {
                count = 0;
                changed = false;
                foreach (NonTerminal nt in thisTestConfig.Where(x=> !x.IsTerminating()))
                {
                    if (nt.productions.Any(x => x.Terminates()))
                    {
                        nt.SetTerminating();
                        changed = true;
                    }
                    if (!nt.IsTerminating())
                        count++;
                }
            }
            while (changed);
            if (count == 0)
                fixes.Add(root);
        }

        private void LeafExperiment(NonTerminal probe, IList<NonTerminal> component)
        {
            // Test what happens with probe terminating ...
            probe.SetTerminating();
            LeafPropagate(probe, component);
            // Then reset the values of all components
            foreach (NonTerminal element in component)
                element.SetTerminating(false);
        }



        private void LeafPropagate(NonTerminal root, IList<NonTerminal> thisTestConfig)
        {
            bool changed = false;
            do
            {
                changed = false;
                foreach (NonTerminal nt in thisTestConfig.Where(x => !x.IsTerminating()))
                {
                    if (nt.productions.Any(p => p.Terminates()))
                    {
                        nt.IsTerminating();
                        changed = true;
                    }
                }
            } while (changed);

            var filtered = thisTestConfig.Where(x => x.IsTerminating());
;
            handler.AddWarning(String.Format(CultureInfo.InvariantCulture,
                        "Terminating {0} fixes the following size-{1} NonTerminal set{2}{{{3}}}",
                        root.ToString(),
                        filtered.Count(),
                        System.Environment.NewLine,
                        ListUtilities.GetStringFromList(filtered)), null);
        }

        // =============================================================================
        #endregion Terminating Computation
        // =============================================================================

        internal bool CheckGrammar(ErrorHandler handler)
        {
            bool ok = true;
            NonTerminal nt;
            this.handler = handler;
            MarkReachable();
            MarkTerminating();
            foreach (var pair in nonTerminals)
            {
                nt = pair.Value;
                if (!nt.reached)
                    handler.AddWarning(String.Format(CultureInfo.InvariantCulture,
                        "NonTerminal symbol \"{0}\" is unreachable", pair.Key), null);

                if (nt.productions.Count == 0)
                {
                    ok = false;
                    handler.AddError(String.Format(CultureInfo.InvariantCulture,
                        "NonTerminal symbol \"{0}\" has no productions", pair.Key), null);
                }
            }
            if (this.HasNonTerminatingNonTerms) ok = false;
            return ok;
        }

        internal void ReportConflicts(StreamWriter wrtr)
        {
            if (wrtr == null)
                return;
            foreach (Conflict theConflict in conflicts)
                theConflict.Report(wrtr);
        }
    }

    #region Conflict Diagnostics

    internal abstract class Conflict
    {
        protected Terminal symbol;
        protected string str1 = null;
        protected string str2 = null;
        internal Conflict(Terminal sy, string s1, string s2) { symbol = sy; str1 = s1; str2 = s2; }

        internal abstract void Report(StreamWriter w);
        internal abstract void HtmlReport(StreamWriter w);
    }

    internal class ReduceReduceConflict : Conflict
    {
        int chosen;
        AutomatonState inState;

        internal ReduceReduceConflict(Terminal sy, string s1, string s2, int prod, AutomatonState state)
            : base(sy, s1, s2)
        {
            chosen = prod;
            inState = state;
            state.Link(this);
        }

        internal override void Report(StreamWriter wrtr)
        {
            wrtr.WriteLine(
                "Reduce/Reduce conflict in state {0} on symbol \"{1}\", parser will reduce production {2}",
                inState.num,
                symbol.ToString(),
                chosen);
            wrtr.WriteLine(str1);
            wrtr.WriteLine(str2);
            wrtr.WriteLine();
        }

        internal override void HtmlReport(StreamWriter wrtr)
        {
            wrtr.WriteLine(
                "      Reduce/Reduce conflict on symbol \"{0}\", parser will reduce production {1}",
                symbol.ToString(),
                chosen);
            wrtr.WriteLine("      " + str1);
            wrtr.WriteLine("      " + str2);
            wrtr.WriteLine("      ---------");
        }
    }

    internal class ShiftReduceConflict : Conflict
    {
        AutomatonState fromState;
        AutomatonState toState;
        internal ShiftReduceConflict(Terminal sy, string s1, string s2, AutomatonState from, AutomatonState to)
            : base(sy, s1, s2)
        {
            fromState = from; toState = to;
            fromState.Link(this);
        }

        internal override void Report(StreamWriter wrtr)
        {
            wrtr.WriteLine("Shift/Reduce conflict on symbol \"{0}\", parser will shift", symbol.ToString());
            wrtr.WriteLine(str1);
            wrtr.WriteLine(str2);
            wrtr.Write("  Items for From-state ");
            wrtr.WriteLine(fromState.ItemDisplay());
            wrtr.Write("  Items for Next-state ");
            wrtr.WriteLine(toState.ItemDisplay());
            wrtr.WriteLine();
        }

        internal override void HtmlReport(StreamWriter wrtr)
        {
            wrtr.WriteLine("      Shift/Reduce conflict on symbol \"{0}\", parser will shift", symbol.ToString());
            wrtr.WriteLine("      " + str1);
            wrtr.WriteLine("      " + str2);
            wrtr.WriteLine("      ---------");
        }

    }

    #endregion

}







