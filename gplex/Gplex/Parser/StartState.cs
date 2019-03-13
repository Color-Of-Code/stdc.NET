// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System.Collections.Generic;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    public sealed class StartState
    {
        private static IdGenerator _ids = new IdGenerator(-1);

        internal IList<RuleDesc> rules = new List<RuleDesc>();

        internal static StartState allState = new StartState("$ALL$", true);    // ord = -1
        internal static StartState initState = new StartState("INITIAL", false); // ord = 0;

        internal StartState(bool isDmy, string str)
        {
            IsDummy = isDmy;
            Name = str;
            Ord = _ids.Next();
        }

        StartState(string str, bool isAll)
        {
            Name = str;
            IsAll = isAll;
            Ord = _ids.Next();
        }

        internal string Name { get; private set; }
        internal int Ord { get; private set; }
        internal bool IsAll { get; private set; }
        internal bool IsDummy { get; private set; }

        internal void AddRule(RuleDesc rule)
        {
            rules.Add(rule);
        }
    }

}
