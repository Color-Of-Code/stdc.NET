

namespace QUT.Gplib
{
    public interface IPrecedence
    {
        PrecedenceType type { get; }
        int prec { get; }
    }
}
