// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using System.Collections.Generic;

namespace QUT.GPGen
{
    /// <summary>
    /// Class for determining input token sequences that
    /// lead to each state by the shortest token sequence.
    /// The corresponding sequence for each NonTerminal is
    /// already computed in Grammar.MarkTerminating() as a
    /// side-effect of detecting non-terminating NonTerms.
    /// </summary>
    internal static class DiagnosticHelp
    {
        private static IList<T> ListClone<T>(IList<T> list)
        {
            IList<T> rslt = new List<T>(list.Count + 1);
            foreach (T item in list)
                rslt.Add(item);
            return rslt;
        }

        internal static void PopulatePrefixes(IList<AutomatonState> states)
        {
            AutomatonState start = states[0];
            start.shortestPrefix = new List<Symbol>(); // The empty list.
            start.statePath = new List<AutomatonState>();
            start.statePath.Add(start);

            bool changed = false;
            do
            {
                changed = false;
                foreach (var state in states)
                {
                    IList<Symbol> newfix;
                    IList<Symbol> prefix = state.shortestPrefix;
                    IList<AutomatonState> newPath;
                    IList<AutomatonState> oldPath = state.statePath;

                    if (prefix != null)
                    {
                        foreach (var a in state.Goto)
                        {
                            Symbol smbl = a.Key;
                            AutomatonState nextState = a.Value;
                            newfix = ListClone<Symbol>(prefix);
                            newPath = ListClone<AutomatonState>(oldPath);

                            newPath.Add(nextState);
                            if (!smbl.IsNullable())
                                newfix.Add(smbl);
                            if (nextState.shortestPrefix == null ||
                                nextState.shortestPrefix.Count > newfix.Count)
                            {
                                nextState.shortestPrefix = newfix;
                                nextState.statePath = newPath;
                                changed = true;
                            }
                        }
                    }
                }
            } while (changed);
        }
    }
}
