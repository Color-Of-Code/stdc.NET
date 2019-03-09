// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System;
using System.Collections.Generic;
using System.Linq;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// This class models the nested start-state scopes.
    /// </summary>
    internal sealed class StartStateScope
    {
        private Stack<IList<StartState>> stack;

        internal StartStateScope()
        {
            stack = new Stack<IList<StartState>>();
            stack.Push(new List<StartState>());
        }

        internal IList<StartState> Current
        {
            get { return stack.Peek(); }
        }

#if STATE_DIAGNOSTICS
        /// <summary>
        /// Diagnostic method for debugging GPLEX
        /// </summary>
        internal void DumpCurrent()
        {
            Console.Write("Current start states: ");
            foreach (StartState elem in Current)
            {
                Console.Write("{0},", elem.Name);
            }
            Console.WriteLine();
        }
#endif

        internal void EnterScope(IList<StartState> list)
        {
            //  There are a couple of tricky cases here.
            //  Neither of them are sensible, but both are
            //  probably legal:
            //  (1)   <*>{
            //            <FOO>{ ...
            //  Active list of start states should be "all states"
            //
            //  (2)   <FOO>{
            //            <*>{ ...
            //  Active list of start states should be "all states"
            //
            IList<StartState> newTop = null;
            var current = this.Current;
            if (list.Contains(StartState.allState))
                newTop = list;
            else if (current.Contains(StartState.allState))
                newTop = current;
            else
            {
                newTop = new List<StartState>(this.Current);
                foreach (StartState elem in list)
                {
                    if (!newTop.Contains(elem))
                        newTop.Add(elem);
                }
            }
            stack.Push(newTop);
        }

        internal void ExitScope()
        {
            if (stack.Count > 1)
                stack.Pop();
        }

        internal void ClearScope()
        {
            if (stack.Count != 1)
            {
                stack = new Stack<IList<StartState>>();
                stack.Push(new List<StartState>());
            }
        }
    }

}
