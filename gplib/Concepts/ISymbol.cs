
namespace QUT.Gplib
{
    public interface ISymbol
    {
        int Number { get; }

        string Kind { get; }

        bool IsNullable();
    }
}
