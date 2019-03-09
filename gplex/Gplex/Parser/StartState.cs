// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System.Collections.Generic;

namespace QUT.Gplex.Parser
{
    internal sealed class StartState
    {
        static int next = -1;

        int ord;
        //bool isExcl;
        //bool isInit;
        bool isAll;
        bool isDummy;
        string name;
        internal List<RuleDesc> rules = new List<RuleDesc>();

        internal static StartState allState = new StartState("$ALL$", true);    // ord = -1
        internal static StartState initState = new StartState("INITIAL", false); // ord = 0;

        internal StartState(bool isDmy, string str)
        {
            isDummy = isDmy; name = str; ord = next++;
        }

        StartState(string str, bool isAll)
        {
            name = str; this.isAll = isAll; ord = next++;
        }

        internal string Name { get { return name; } }
        internal int Ord { get { return ord; } }
        internal bool IsAll { get { return isAll; } }
        internal bool IsDummy { get { return isDummy; } }

        internal void AddRule(RuleDesc rule)
        {
            rules.Add(rule);
        }
    }

}
