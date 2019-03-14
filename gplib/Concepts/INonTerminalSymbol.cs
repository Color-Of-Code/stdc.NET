
namespace QUT.Gplib
{
    public interface INonTerminalSymbol : ISymbol
    {
        bool reached { get; set; }
    }
}
