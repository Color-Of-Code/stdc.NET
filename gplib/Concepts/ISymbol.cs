
namespace QUT.Gplib
{
    public interface ISymbol
    {
        string Name { get; }

        int Id { get; }

        string Kind { get; }

        bool IsNullable();

        bool IsTerminating();
    }
}
