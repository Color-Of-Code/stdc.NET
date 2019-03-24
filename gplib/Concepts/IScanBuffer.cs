
namespace QUT.Gplib
{
    public interface IScanBuffer
    {
        int Pos { get; set; }
        string GetString(int startPosition, int endPosition);
        int Read();
        void Mark();
    }
}
