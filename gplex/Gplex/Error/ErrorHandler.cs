// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using QUT.GplexBuffers;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    internal class ErrorHandler
    {
        const int maxErrors = 100; // Will this be enough for all users?

        private List<Error> _errors;

        internal bool HasErrors { get { return _errors.Any(x => !x.IsWarning); } }
        internal bool HasWarnings { get { return _errors.Any(x => x.IsWarning); } }


        internal int ErrorCount { get { return _errors.Count(x => !x.IsWarning); } }
        internal int WarningCount { get { return _errors.Count(x => x.IsWarning); } }

        internal ErrorHandler()
        {
            _errors = new List<Error>(8);
        }
        // -----------------------------------------------------
        //   Public utility methods
        // -----------------------------------------------------

        internal IEnumerable<Error> SortedErrorList()
        {
            if (_errors.Count > 1) _errors.Sort();
            return _errors;
        }

        internal void AddError(string msg, ISpan spn)
        {
            this.AddError(new Error(3, msg, spn, false));
        }

        private void AddError(Error e)
        {
            _errors.Add(e);
            if (_errors.Count > maxErrors)
            {
                _errors.Add(new Error(2, "Too many errors, abandoning", e.Span, false));
                throw new TooManyErrorsException("Too many errors");
            }
        }

        /// <summary>
        /// Add this error to the error buffer.
        /// </summary>
        /// <param name="spn">The span to which the error is attached</param>
        /// <param name="num">The error number</param>
        /// <param name="key">The featured string</param>
        internal void ListError(ISpan spn, int num, string key, char quote)
        { ListError(spn, num, key, quote, quote); }
        internal void ListError(ISpan spn, int num, string key)
        { ListError(spn, num, key, '<', '>'); }

        void ListError(ISpan spn, int num, string key, char lh, char rh)
        {
            string prefix, suffix, message;
            switch (num)
            {
                case 1:
                    prefix = "Parser error"; suffix = "";
                    break;
                case 50:
                    prefix = "Start state"; suffix = "already defined";
                    break;
                case 51:
                    prefix = "Start state"; suffix = "undefined";
                    break;
                case 52:
                    prefix = "Lexical category"; suffix = "already defined";
                    break;
                case 53:
                    prefix = "Expected character"; suffix = "";
                    break;
                case 55:
                    prefix = "Unknown lexical category"; suffix = "";
                    break;
                case 61:
                    prefix = "Missing matching construct"; suffix = "";
                    break;
                case 62:
                    prefix = "Unexpected symbol, skipping to "; suffix = "";
                    break;
                case 70:
                    prefix = "Illegal character escape "; suffix = "";
                    break;
                case 71:
                    prefix = "Lexical category must be a character class "; suffix = "";
                    break;
                case 72:
                    prefix = "Illegal name for start condition "; suffix = "";
                    break;
                case 74:
                    prefix = "Unrecognized \"%option\" command "; suffix = "";
                    break;
                case 76:
                    prefix = "Unknown character predicate"; suffix = "";
                    break;
                case 83:
                    prefix = "Cannot set /unicode option inconsistently"; suffix = "";
                    break;
                case 84:
                    prefix = "Inconsistent \"%option\" command "; suffix = "";
                    break;
                case 85:
                    prefix = "Unicode literal too large:"; suffix = "use %option unicode";
                    break;
                case 86:
                    prefix = "Illegal octal character escape "; suffix = "";
                    break;
                case 87:
                    prefix = "Illegal hexadecimal character escape "; suffix = "";
                    break;
                case 88:
                    prefix = "Illegal unicode character escape "; suffix = "";
                    break;
                case 96:
                    prefix = "Class"; suffix = "not found in assembly";
                    break;
                case 97:
                    prefix = "Method"; suffix = "not found in class";
                    break;
                case 99:
                    prefix = "Illegal escape sequence"; suffix = "";
                    break;
                case 103:
                    prefix = "Expected character with property"; suffix = "";
                    break;

                // Warnings ...

                case 111:
                    prefix = "This char"; suffix = "does not need escape in character class";
                    break;
                case 113:
                    prefix = "Special case:"; suffix = "included as set class member";
                    break;
                case 114:
                    prefix = "No upper bound to range,"; suffix = "included as set class members";
                    break;
                case 116: prefix = "This pattern always overridden by"; suffix = ""; break;
                case 117: prefix = "This pattern always overrides"; suffix = ""; break;
                case 121: prefix = "char class"; suffix = ""; break;

                default:
                    prefix = "Error " + Convert.ToString(num, CultureInfo.InvariantCulture); suffix = "";
                    break;
            }
            // message = prefix + " <" + key + "> " + suffix;
            message = String.Format(CultureInfo.InvariantCulture, "{0} {1}{2}{3} {4}", prefix, lh, key, rh, suffix);
            this.AddError(new Error(num, message, spn, num >= 110));
        }

        internal void ListError(ISpan spn, int num)
        {
            string message;
            switch (num)
            {
                case 54: message = "Invalid character range: lower bound > upper bound"; break;
                case 56: message = "\"using\" is illegal, use \"%using\" instead"; break;
                case 57: message = "\"namespace\" is illegal, use \"%namespace\" instead"; break;
                case 58: message = "Type declarations impossible in this context"; break;
                case 59: message = "\"next\" action '|' cannot be used on last pattern"; break;
                case 60: message = "Unterminated block comment starts here"; break;
                case 63: message = "Invalid single-line action"; break;
                case 64: message = "This token unexpected"; break;
                case 65: message = "Invalid action"; break;
                case 66: message = "Missing comma in namelist"; break;
                case 67: message = "Invalid or empty namelist"; break;
                case 68: message = "Invalid production rule"; break;
                case 69: message = "Symbols '^' and '$' can only occur at the ends of patterns"; break;

                case 73: message = "No namespace has been defined"; break;
                case 75: message = "Context must have fixed right length or fixed left length"; break;
                case 77: message = "Unknown LEX tag name"; break;
                case 78: message = "Expected space here"; break;
                case 79: message = "Illegal character in this context"; break;

                case 80: message = "Expected end-of-line here"; break;
                case 81: message = "Invalid character range, no upper bound character"; break;
                case 82: message = "Invalid class character: '-' must be \\escaped"; break;
                case 89: message = "Empty semantic action, must be at least a comment"; break;

                case 90: message = "\"%%\" marker must start at beginning of line"; break;
                case 91: message = "Version of gplexx.frame is not recent enough"; break;
                case 92: message = "Non unicode scanners only allow single-byte codepages"; break;
                case 93: message = "Non unicode scanners cannot use /codepage:guess"; break;
                case 94: message = "This assembly could not be found"; break;
                case 95: message = "This assembly could not be loaded"; break;
                case 98: message = "Only \"public\" or \"internal\" allowed here"; break;

                case 100: message = "Context operator cannot be used with right anchor '$'"; break;
                case 101: message = "Extra characters at end of regular expression"; break;
                case 102: message = "Literal string terminated by end of line"; break;

                // Warnings ...

                case 110: message = "Code between rules, ignored"; break;
                case 112: message = "/babel option is unsafe without /unicode option"; break;
                case 115: message = "This pattern matches \"\", and might loop"; break;
                case 116: message = "This pattern is never matched"; break;
                case 118: message = "This constructed set is empty"; break;

                default: message = "Error " + Convert.ToString(num, CultureInfo.InvariantCulture); break;
            }
            this.AddError(new Error(num, message, spn, num >= 110));
        }


        // -----------------------------------------------------
        //   Error Listfile Reporting Method
        // -----------------------------------------------------

        internal void MakeListing(ScanBuff buff, StreamWriter streamWriter, string name, string version)
        {
            int line = 1;
            int eNum = 0;
            int eLin = 0;

            int nxtC = (int)'\n';
            int groupFirst;
            int currentCol;
            int currentLine;

            //
            //  Errors are sorted by line number
            //
            _errors = SortedErrorList().ToList();
            //
            //  Reset the source file buffer to the start
            //
            buff.Pos = 0;
            streamWriter.WriteLine();
            ListDivider(streamWriter);
            streamWriter.WriteLine("//  GPLEX error listing for lex source file <"
                                                           + name + ">");
            ListDivider(streamWriter);
            streamWriter.WriteLine("//  Version:  " + version);
            streamWriter.WriteLine("//  Machine:  " + Environment.MachineName);
            //streamWriter.WriteLine("//  DateTime: " + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            streamWriter.WriteLine("//  UserName: " + Environment.UserName);
            ListDivider(streamWriter); streamWriter.WriteLine(); streamWriter.WriteLine();
            //
            //  Initialize the error group
            //
            groupFirst = 0;
            currentCol = 0;
            currentLine = 0;
            //
            //  Now, for each error do
            //
            for (eNum = 0; eNum < _errors.Count; eNum++)
            {
                Error errN = _errors[eNum];
                eLin = errN.Span.StartLine;
                if (eLin > currentLine)
                {
                    //
                    // Spill all the waiting messages
                    //
                    int maxGroupWidth = 0;
                    if (currentCol > 0)
                    {
                        streamWriter.WriteLine();
                        currentCol = 0;
                    }
                    for (int i = groupFirst; i < eNum; i++)
                    {
                        Error err = _errors[i];
                        string prefix = (err.IsWarning ? "// Warning: " : "// Error: ");
                        string msg = StringUtilities.MakeComment(3, prefix + err.Message);
                        if (StringUtilities.MaxWidth(msg) > maxGroupWidth)
                            maxGroupWidth = StringUtilities.MaxWidth(msg);
                        streamWriter.Write(msg);
                        streamWriter.WriteLine();
                    }
                    if (groupFirst < eNum)
                    {
                        streamWriter.Write("// ");
                        Spaces(streamWriter, maxGroupWidth - 3);
                        streamWriter.WriteLine();
                    }
                    currentLine = eLin;
                    groupFirst = eNum;
                }
                //
                //  Emit lines up to *and including* the error line
                //
                while (line <= eLin)
                {
                    nxtC = buff.Read();
                    if (nxtC == (int)'\n')
                        line++;
                    else if (nxtC == ScanBuff.EndOfFile)
                        break;
                    streamWriter.Write((char)nxtC);
                }
                //
                //  Now emit the error message(s)
                //
                if (errN.Span.EndColumn > 3 && errN.Span.StartColumn < 80)
                {
                    if (currentCol == 0)
                    {
                        streamWriter.Write("//");
                        currentCol = 2;
                    }
                    if (errN.Span.StartColumn > currentCol)
                    {
                        Spaces(streamWriter, errN.Span.StartColumn - currentCol);
                        currentCol = errN.Span.StartColumn;
                    }
                    for (; currentCol < errN.Span.EndColumn && currentCol < 80; currentCol++)
                        streamWriter.Write('^');
                }
            }
            //
            //  Clean up after last message listing
            //  Spill all the waiting messages
            //
            int maxEpilogWidth = 0;
            if (currentCol > 0)
            {
                streamWriter.WriteLine();
            }
            for (int i = groupFirst; i < _errors.Count; i++)
            {
                Error err = _errors[i];
                string prefix = (err.IsWarning ? "// Warning: " : "// Error: ");
                string msg = StringUtilities.MakeComment(3, prefix + err.Message);
                if (StringUtilities.MaxWidth(msg) > maxEpilogWidth)
                    maxEpilogWidth = StringUtilities.MaxWidth(msg);
                streamWriter.Write(msg);
                streamWriter.WriteLine();
            }
            if (groupFirst < _errors.Count)
            {
                streamWriter.Write("// ");
                Spaces(streamWriter, maxEpilogWidth - 3);
                streamWriter.WriteLine();
            }
            //
            //  And dump the tail of the file
            //
            nxtC = buff.Read();
            while (nxtC != ScanBuff.EndOfFile)
            {
                streamWriter.Write((char)nxtC);
                nxtC = buff.Read();
            }
            ListDivider(streamWriter); streamWriter.WriteLine();
            streamWriter.Flush();
            // sWrtr.Close();
        }

        private static void ListDivider(StreamWriter wtr)
        {
            wtr.WriteLine(
            "// =========================================================================="
            );
        }

        private static void Spaces(StreamWriter wtr, int len)
        {
            for (int i = 0; i < len; i++) wtr.Write('-');
        }


        // -----------------------------------------------------
        //   Console Error Reporting Method
        // -----------------------------------------------------

        internal void DumpErrorsInMsbuildFormat(ScanBuff buff, TextWriter wrtr)
        {
            var builder = new StringBuilder();
            //
            // Message prefix
            //
            string location = (buff != null ? buff.FileName : "GPLEX");
            foreach (var err in _errors)
            {
                builder.Length = 0; // Works for V2.0 even.
                //
                // Origin
                //
                builder.Append(location);
                if (buff != null)
                {
                    builder.Append('(');
                    builder.Append(err.Span.StartLine);
                    builder.Append(',');
                    builder.Append(err.Span.StartColumn);
                    builder.Append(')');
                }
                builder.Append(':');
                //
                // Category                builder.Append( ':' );
                //
                builder.Append(err.IsWarning ? "warning " : "error ");
                builder.Append(err.Code);
                builder.Append(':');
                //
                // Message
                //
                builder.Append(err.Message);
                Console.Error.WriteLine(builder.ToString());
            }
        }



        internal void DumpAll(ScanBuff buff, TextWriter wrtr)
        {
            int line = 1;
            int eNum = 0;
            int eLin = 0;
            int nxtC = (int)'\n';
            //
            //  Initialize the error group
            //
            int groupFirst = 0;
            int currentCol = 0;
            int currentLine = 0;
            //
            //  Reset the source file buffer to the start
            //
            buff.Pos = 0;
            wrtr.WriteLine("Error Summary --- ");
            //
            //  Initialize the error group
            //
            groupFirst = 0;
            currentCol = 0;
            currentLine = 0;
            //
            //  Now, for each error do
            //
            for (eNum = 0; eNum < _errors.Count; eNum++)
            {
                eLin = _errors[eNum].Span.StartLine;
                if (eLin > currentLine)
                {
                    //
                    // Spill all the waiting messages
                    //
                    if (currentCol > 0)
                    {
                        wrtr.WriteLine();
                        currentCol = 0;
                    }
                    for (int i = groupFirst; i < eNum; i++)
                    {
                        Error err = _errors[i];
                        wrtr.Write((err.IsWarning ? "Warning: " : "Error: "));
                        wrtr.Write(err.Message);
                        wrtr.WriteLine();
                    }
                    currentLine = eLin;
                    groupFirst = eNum;
                }
                //
                //  Skip lines up to *but not including* the error line
                //
                while (line < eLin)
                {
                    nxtC = buff.Read();
                    if (nxtC == (int)'\n') line++;
                    else if (nxtC == ScanBuff.EndOfFile) break;
                }
                //
                //  Emit the error line
                //
                if (line <= eLin)
                {
                    wrtr.Write((char)((eLin / 1000) % 10 + (int)'0'));
                    wrtr.Write((char)((eLin / 100) % 10 + (int)'0'));
                    wrtr.Write((char)((eLin / 10) % 10 + (int)'0'));
                    wrtr.Write((char)((eLin) % 10 + (int)'0'));
                    wrtr.Write(' ');
                    while (line <= eLin)
                    {
                        nxtC = buff.Read();
                        if (nxtC == (int)'\n') line++;
                        else if (nxtC == ScanBuff.EndOfFile) break;
                        wrtr.Write((char)nxtC);
                    }
                }
                //
                //  Now emit the error message(s)
                //
                if (_errors[eNum].Span.StartColumn >= 0 && _errors[eNum].Span.StartColumn < 75)
                {
                    if (currentCol == 0)
                    {
                        wrtr.Write("-----");
                    }
                    for (int i = currentCol; i < _errors[eNum].Span.StartColumn; i++, currentCol++)
                    {
                        wrtr.Write('-');
                    }
                    for (; currentCol < _errors[eNum].Span.EndColumn && currentCol < 75; currentCol++)
                        wrtr.Write('^');
                }
            }
            //
            //  Clean up after last message listing
            //  Spill all the waiting messages
            //
            if (currentCol > 0)
            {
                wrtr.WriteLine();
            }
            for (int i = groupFirst; i < _errors.Count; i++)
            {
                Error err = _errors[i];
                wrtr.Write((err.IsWarning ? "Warning: " : "Error: "));
                wrtr.Write(_errors[i].Message);
                wrtr.WriteLine();
            }
        }
    }
}
