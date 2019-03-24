
namespace QUT.Gplib
{
    public interface IScanBuffer
    {
        string FileName { get; set; }
        int Pos { get; set; }
        string GetString(int startPosition, int endPosition);
        int Read();
        void Mark();
    }
}
