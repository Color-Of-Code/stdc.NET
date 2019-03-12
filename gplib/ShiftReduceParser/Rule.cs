// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)


namespace QUT.Gplib
{
    /// <summary>
    /// Rule representation at runtime.
    /// </summary>
    public class Rule : IProductionRule
    {
        internal int LeftHandSide; // symbol
        internal int[] RightHandSide; // symbols

        /// <summary>
        /// Rule constructor.  This holds the ordinal of
        /// the left hand side symbol, and the list of
        /// right hand side symbols, in lexical order.
        /// </summary>
        /// <param name="left">The LHS non-terminal</param>
        /// <param name="right">The RHS symbols, in lexical order</param>
        public Rule(int left, int[] right)
        {
            this.LeftHandSide = left;
            this.RightHandSide = right;
        }
    }
}
