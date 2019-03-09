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
        internal bool symbolic;
        private string alias;

        internal string Alias { get { return alias; } }

        internal override int num
        {
            get
            {
                if (symbolic)
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
		internal Terminal(bool symbolic, string name)
            : base(name)
        {
            this.symbolic = symbolic;
            if (symbolic)
                this.n = ++count;
            else
            {
                this.n = CharacterUtilities.OrdinalOfCharacterLiteral(name, 1);
                if (n > max) max = n;
            }
        }

        internal Terminal(bool symbolic, string name, string alias)
            : this(symbolic, name)
        {
            if (alias != null)
                this.alias = alias;
        }


        internal override bool IsNullable()
        {
            return false;
        }

        public override string ToString()
        {
            return this.alias ?? base.ToString();
        }

        internal string EnumName()
        {
            return base.ToString();
        }
    }
}