namespace QUT.Gplib
{
    /// <summary>
    /// This class reads characters from a single string as
    /// required, for example, by Visual Studio language services
    /// </summary>
    internal sealed class StringBuffer : IScanBuffer
    {
        // input buffer
        private readonly string _str;
        private readonly int _len;

        internal StringBuffer(string source)
        {
            _str = source;
            _len = source.Length;
        }

        public string FileName { get { return null; } }

        public void Mark()
        {}

        public int Read()
        {
            if (Pos < _len) return _str[Pos++];
            else if (Pos == _len) { Pos++; return '\n'; }   // one strike, see new line
            else { Pos++; return ScanBuffCode.EndOfFile; }                // two strikes and you're out!
        }

        public string GetString(int begin, int limit)
        {
            //  "limit" can be greater than sLen with the BABEL
            //  option set.  Read returns a "virtual" EOL if
            //  an attempt is made to read past the end of the
            //  string buffer.  Without the guard any attempt 
            //  to fetch yytext for a token that includes the 
            //  EOL will throw an index exception.
            if (limit > _len) limit = _len;
            if (limit <= begin) return "";
            else return _str.Substring(begin, limit - begin);
        }

        public int Pos
        {
            get;
            set;
        }

        public override string ToString() { return "StringBuffer"; }
    }
}
