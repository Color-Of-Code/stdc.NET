// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using QUT.Gplib;

namespace QUT.GPGen
{
    internal class Terminal : Symbol
    {
        static int count;
        static int max;
        internal static int Max { get { return max; } }

        internal Precedence prec;
        private int n;
        internal bool IsSymbolic { get; private set; }

        internal string Alias { get; private set; }

        internal override int num
        {
            get
            {
                if (IsSymbolic)
                    return max + n;
                else
                    return n;
            }
        }

        /// <summary>
        /// If name is an escaped char-lit, it must already be
        /// canonicalized according to some convention. In this 
        /// application CharUtils.Canonicalize().
        /// </summary>
        /// <param name="symbolic">Means "is an ident"</param>
        /// <param name="name">string representation of symbol</param>
		internal Terminal(bool symbolic, string name, string alias = null)
            : base(name)
        {
            IsSymbolic = symbolic;
            if (symbolic)
                this.n = ++count;
            else
            {
                this.n = CharacterUtilities.OrdinalOfCharacterLiteral(name, 1);
                if (n > max) max = n;
            }
            Alias = alias;
        }

        internal override bool IsNullable()
        {
            return false;
        }

        public override string ToString()
        {
            return Alias ?? base.ToString();
        }

        internal string EnumName()
        {
            return base.ToString();
        }
    }
}