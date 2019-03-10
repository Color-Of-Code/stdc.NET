// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex.Parser
{
    /// <summary>
    /// Abstract class for AST representing regular expressions.
    /// Concrete subclasses correspond to --- 
    /// binary trees (context, alternation and concatenation)
    /// unary trees (closure, finite repetition and anchored patterns)
    /// leaf nodes (chars, char classes, literal strings and the eof marker)
    /// </summary>
    internal abstract class RegExTree
    {
        public RegOp Operator { get; private set; }

        internal RegExTree(RegOp op) { this.Operator = op; }

        /// <summary>
        /// This is a helper to compute the length of strings
        /// recognized by a regular expression.  This is important
        /// because the right context operator "R1/R2" is efficiently 
        /// implemented if either R1 or R2 produce fixed length strings.
        /// </summary>
        /// <returns>0 if length is variable, otherwise length</returns>
        internal abstract int ContextLength();

        /// <summary>
        /// This is a helper to compute the minimum length of
        /// strings recognized by a regular expression.  It is the
        /// minimum consumption of input by a pattern.
        /// </summary>
        /// <returns>Minimum length of pattern</returns>
        internal abstract int MinimumLength();

        /// <summary>
        /// This is the navigation method for running the visitor
        /// over the tree in a depth-first-search visit order.
        /// </summary>
        /// <param name="visitor">visitor.Op(this) is called on each node</param>
        internal abstract void Visit(RegExDFS visitor);

        internal virtual bool HasRightContext { get { return false; } }
    }
}
