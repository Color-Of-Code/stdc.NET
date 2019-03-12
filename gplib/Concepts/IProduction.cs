
using System.Collections.Generic;

namespace QUT.Gplib
{
    public interface IProduction
    {
        // INonTerminalSymbol lhs { get; }
        IList<ISymbol> rhs { get; }
    }
}
