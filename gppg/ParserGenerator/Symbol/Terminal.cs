// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2010
// (see accompanying GPPGcopyright.rtf)

using System;
using QUT.Gplib;

namespace QUT.GPGen
{
    internal class Terminal : Symbol
    {
        // TODO: these globals hinder running parsers in parallel
        private static int _count;
        internal static int MaxOrdinalOfCharacterLiteral { get; private set; }

        internal Precedence prec;
        private int _number;
        internal bool IsSymbolic { get; private set; }

        internal string Alias { get; private set; }

        internal override int Number
        {
            get
            {
                if (IsSymbolic)
                    return MaxOrdinalOfCharacterLiteral + _number;
                else
                    return _number;
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
                this._number = ++_count;
            else
            {
                this._number = CharacterUtilities.OrdinalOfCharacterLiteral(name, 1);
                MaxOrdinalOfCharacterLiteral = Math.Max(_number, MaxOrdinalOfCharacterLiteral);
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