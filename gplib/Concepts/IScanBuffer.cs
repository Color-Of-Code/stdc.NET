
namespace QUT.Gplib
{
    public interface IScanBuffer
    {
        string FileName { get; }
        int Pos { get; set; }
        string GetString(int startPosition, int endPosition);
        int Read();
        void Mark();
    }
}
