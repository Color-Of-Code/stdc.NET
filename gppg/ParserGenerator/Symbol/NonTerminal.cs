
// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)


using System.Collections.Generic;


namespace QUT.GPGen
{
    internal class NonTerminal : Symbol
    {
        internal bool reached;

        // Start experimental features
        internal IList<NonTerminal> dependsOnList;
        internal int depth;
        internal bool terminating;
        // end

        static int count;
        private int n;
        internal IList<Production> productions = new List<Production>();

        internal NonTerminal(string name)
            : base(name)
        {
            n = ++count;
        }

        internal override int num
        {
            get
            {
                return -n;
            }
        }

        private object isNullable;
        internal override bool IsNullable()
        {
            if (isNullable == null)
            {
                isNullable = false;
                foreach (var p in productions)
                {
                    bool nullable = true;
                    foreach (Symbol rhs in p.rhs)
                        if (!rhs.IsNullable())
                        {
                            nullable = false;
                            break;
                        }
                    if (nullable)
                    {
                        isNullable = true;
                        break;
                    }
                }
            }

            return (bool)isNullable;
        }
    }
}