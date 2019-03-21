namespace QUT.Gplib
{
    // ==============================================================
    // =====  Definitions for various ScanBuff derived classes   ====
    // ==============================================================
    // ===============         String input          ================
    // ==============================================================

    /// <summary>
    /// This class reads characters from a single string as
    /// required, for example, by Visual Studio language services
    /// </summary>
    sealed class StringBuffer : ScanBuff
    {
        string str;        // input buffer
        int bPos;          // current position in buffer
        int sLen;

        public StringBuffer(string source)
        {
            this.str = source;
            this.sLen = source.Length;
            this.FileName = null;
        }

        public override int Read()
        {
            if (bPos < sLen) return str[bPos++];
            else if (bPos == sLen) { bPos++; return '\n'; }   // one strike, see new line
            else { bPos++; return ScanBuffCode.EndOfFile; }                // two strikes and you're out!
        }

        public override string GetString(int begin, int limit)
        {
            //  "limit" can be greater than sLen with the BABEL
            //  option set.  Read returns a "virtual" EOL if
            //  an attempt is made to read past the end of the
            //  string buffer.  Without the guard any attempt 
            //  to fetch yytext for a token that includes the 
            //  EOL will throw an index exception.
            if (limit > sLen) limit = sLen;
            if (limit <= begin) return "";
            else return str.Substring(begin, limit - begin);
        }

        public override int Pos
        {
            get { return bPos; }
            set { bPos = value; }
        }

        public override string ToString() { return "StringBuffer"; }
    }
}
