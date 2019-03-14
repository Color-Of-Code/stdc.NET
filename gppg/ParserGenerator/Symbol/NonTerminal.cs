
// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)


using System;
using System.Collections.Generic;
using System.Linq;
using QUT.Gplib;

namespace QUT.GPGen
{
    public class NonTerminal : Symbol, INonTerminalSymbol
    {
        public bool reached { get; set; }

        // Start experimental features
        internal IList<NonTerminal> dependsOnList;
        internal int depth;
        // end

        private static IdGenerator _ids = new IdGenerator(1);
        private int n;
        internal IList<Production> productions = new List<Production>();

        internal NonTerminal(string name)
            : base(name)
        {
            n = _ids.Next();
        }

        public override int Id
        {
            get
            {
                return -n;
            }
        }

        private bool? _isNullable;
        public override bool IsNullable()
        {
            if (_isNullable.HasValue)
                return _isNullable.Value;
            // needed because of recursive calls
            _isNullable = false;

            _isNullable = productions.Any(p => p.IsNullable());
            return _isNullable.Value;
        }

        private bool _isTerminating;
        public override bool IsTerminating()
        {
            return _isTerminating;
        }

        internal void SetTerminating(bool v = true)
        {
            _isTerminating = v;
        }
    }
}