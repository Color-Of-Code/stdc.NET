//
// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)
// 
// These utilities are used in GPLEX and GPPG
//

using System;

namespace QUT.Gplex.Parser
{
    public static class StringUtilities
    {
        /// <summary>
        /// Modifies a string so that if it contains multiple lines
        /// every line after the first is prepended by "//" followed
        /// by "indent" spaces.
        /// </summary>
        /// <param name="indent">The line indent</param>
        /// <param name="text">The input string</param>
        /// <returns>The modified string</returns>
        public static string MakeComment(int indent, string text)
        {
            string EOLmark = System.Environment.NewLine + new String(' ', indent) + "// ";
            return text.Replace(System.Environment.NewLine, EOLmark);
        }

        /// <summary>
        /// Returns the character width of a (possibly multiline) string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int MaxWidth(string source)
        {
            int rslt = 0;
            string[] lines = source.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
                if (line.Length > rslt)
                    rslt = line.Length;
            return rslt;
        }
    }
}