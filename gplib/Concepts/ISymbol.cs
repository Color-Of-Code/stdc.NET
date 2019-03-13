
namespace QUT.Gplib
{
    public interface ISymbol
    {
        string Name { get; }

        int Number { get; }

        string Kind { get; }

        bool IsNullable();
    }
}
