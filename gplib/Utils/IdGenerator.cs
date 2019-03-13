
namespace QUT.Gplib
{
    public class IdGenerator
    {
        private int _count;
        
        public IdGenerator(int from = 0)
        {
            _count = from;
        }

        public int Next()
        {
            return _count++;
        }
    }
}
