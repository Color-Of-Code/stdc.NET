
namespace QUT.Gplib
{
    public interface ITerminalSymbol : ISymbol
    {
        IPrecedence prec { get; set; }
    }
}
