// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, K John Gough, QUT 2006-2010
// (see accompanying GPPGcopyright.rtf)
// This file author: John Gough, borrowed from GPLEX

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using QUT.GplexBuffers;
using QUT.Gplib;

namespace QUT.GPGen.Parser
{


    internal class ErrorHandler
    {
        const int MaxErrors = 100; // Will this be enough for all users?

        // TODO: avoid internal set
        internal ISpan DefaultSpan { get; set; }

        private List<Error> _errors;

        internal bool HasErrors { get { return _errors.Any(x => !x.IsWarning); } }
        internal bool HasWarnings { get { return _errors.Any(x => x.IsWarning); } }


        internal int ErrorCount { get { return _errors.Count(x => !x.IsWarning); } }
        internal int WarningCount { get { return _errors.Count(x => x.IsWarning); } }

        internal ErrorHandler()
        {
            _errors = new List<Error>(8);
        }

        private void AddError(Error e) {
            _errors.Add(e);
            if (_errors.Count > MaxErrors) {
                _errors.Add(new Error("Too many errors, abandoning", e.Span, false));
                throw new TooManyErrorsException("Too many errors");
            }
        }
 
        // -----------------------------------------------------
        //   Public utility methods
        // -----------------------------------------------------

        internal List<Error> SortedErrorList()
        {
            if (_errors.Count > 1) _errors.Sort();
            return _errors;
        }

        internal void AddError(string msg, ISpan spn)
        {
            if (spn == null)
                spn = DefaultSpan;
            this.AddError(new Error(msg, spn, false));
        }

        internal void AddWarning(string msg, ISpan spn)
        {
            if (spn == null)
                spn = DefaultSpan;
            this.AddError(new Error(msg, spn, true));
        }

        /// <summary>
        /// Add this error to the error buffer.
        /// </summary>
        /// <param name="spn">The span to which the error is attached</param>
        /// <param name="num">The error number</param>
        /// <param name="key">The featured string</param>
        internal void ListError(ISpan spn, int num, string key, char quote)
        { ListError(spn, num, key, quote, quote); }

        void ListError(ISpan spn, int num, string key, char lh, char rh)
        {
            string prefix, suffix, message;
            if (spn == null)
                spn = DefaultSpan;
            switch (num)
            {
                // Syntactic Errors Detected by the Parser ...
                case 70: prefix = "Invalid string escape"; suffix = ""; break;
                case 103: prefix = "Highest char literal token"; suffix = "is very large"; break;

                default: prefix = "Error " + Convert.ToString(num, CultureInfo.InvariantCulture); suffix = "";
                    break;
            }
            message = String.Format(CultureInfo.InvariantCulture, "{0} {1}{2}{3} {4}", prefix, lh, key, rh, suffix);
            this.AddError(new Error(message, spn, num >= 100));
        }


        internal void ListError(ISpan spn, int num)
        {
            string message;
            switch (num)
            {
                // Lexical Errors Detected by the Scanner ...
                case 50: message = "Unknown %keyword in this context"; break;
                case 51: message = "Bad format for decimal number"; break;
                case 52: message = "Bad format for hexadecimal number"; break;
                case 53: message = "Unterminated comment starts here"; break;
                case 54: message = "Only whitespace is permitted here"; break;
                case 55: message = "Code block has unbalanced braces '{','}'"; break;
                case 56: message = "Keyword \"%}\" is out of place here"; break;
                case 57: message = "This character is invalid in this context"; break;
                case 58: message = "Literal string terminated by EOL"; break;
                case 59: message = "Keyword must start in column-0"; break;
                case 60: message = "Premature termination of Code Block"; break;

                // Syntactic Errors Detected by the Parser ...
                case 71: message = "With %union, %YYSTYPE can only be a simple name"; break;
                case 72: message = "Duplicate definition of Semantic Value Type name"; break;
                case 73: message = "Semantic action index is out of bounds"; break;
                case 74: message = "Unknown special marker in semantic action"; break;
                case 75: message = "Bad separator character in list"; break;
                case 76: message = "This name already defined as a terminal symbol"; break;
                case 77: message = "Position of unmatched brace"; break;
                case 78: message = "Literal string terminated by end of line"; break;

                // Warnings Issued by Either Scanner or Parser ...
                case 100: message = "Optional numeric code ignored in this version"; break;
                case 101: message = "%locations is the default in GPPG"; break;
                case 102: message = "Mid-rule %prec has no effect"; break;

                default: message = "Error " + Convert.ToString(num, CultureInfo.InvariantCulture); break;
            }
            this.AddError(new Error(message, spn, num >= 100));
        }
 
        
        // -----------------------------------------------------
        //   Error Listfile Reporting Method
        // -----------------------------------------------------

        internal void MakeListing(ScanBuff buff, StreamWriter sWrtr, string name, string version)
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
            _errors = SortedErrorList();
            //
            //  Reset the source file buffer to the start
            //
            buff.Pos = 0;
            sWrtr.WriteLine(); 
            ListDivider(sWrtr);
            sWrtr.WriteLine("//  GPPG error listing for yacc source file <"
                                                           + name + ">");
            ListDivider(sWrtr);
            sWrtr.WriteLine("//  Version:  " + version);
            sWrtr.WriteLine("//  Machine:  " + Environment.MachineName);
            sWrtr.WriteLine("//  DateTime: " + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            sWrtr.WriteLine("//  UserName: " + Environment.UserName);
            ListDivider(sWrtr); sWrtr.WriteLine(); sWrtr.WriteLine();
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
                eLin = errN.Span.startLine;
                if (eLin > currentLine)
                {
                    //
                    // Spill all the waiting messages
                    //
                    int maxGroupWidth = 0;
                    if (currentCol > 0)
                    {
                        sWrtr.WriteLine();
                        currentCol = 0;
                    }
                    for (int i = groupFirst; i < eNum; i++)
                    {
                        Error err = _errors[i];
                        string prefix = (err.IsWarning ? "// Warning: " : "// Error: ");
                        string msg = StringUtilities.MakeComment(3, prefix + err.Message);
                        if (StringUtilities.MaxWidth(msg) > maxGroupWidth)
                            maxGroupWidth = StringUtilities.MaxWidth(msg);
                        sWrtr.Write(msg);
                        sWrtr.WriteLine();
                    }
                    if (groupFirst < eNum)
                    {
                        sWrtr.Write("// ");
                        Spaces(sWrtr, maxGroupWidth - 3);
                        sWrtr.WriteLine();
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
                    sWrtr.Write((char)nxtC);
                }
                //
                //  Now emit the error message(s)
                //
                if (errN.Span.endColumn > 3 && errN.Span.startColumn < 80)
                {
                    if (currentCol == 0)
                    {
                        sWrtr.Write("//");
                        currentCol = 2;
                    }
                    if (errN.Span.startColumn > currentCol)
                    {
                        Spaces(sWrtr, errN.Span.startColumn - currentCol - 1);
                        currentCol = errN.Span.startColumn - 1;
                    }
                    for (; currentCol < errN.Span.endColumn && currentCol < 80; currentCol++ )
                        sWrtr.Write('^'); 
                }
            }
            //
            //  Clean up after last message listing
            //  Spill all the waiting messages
            //
            int maxEpilogWidth = 0;
            if (currentCol > 0)
            {
                sWrtr.WriteLine();
            }
            for (int i = groupFirst; i < _errors.Count; i++)
            {
                Error err = _errors[i];
                string prefix = (err.IsWarning ? "// Warning: " : "// Error: ");
                string msg = StringUtilities.MakeComment(3, prefix + err.Message);
                if (StringUtilities.MaxWidth(msg) > maxEpilogWidth)
                    maxEpilogWidth = StringUtilities.MaxWidth(msg);
                sWrtr.Write(msg);
                sWrtr.WriteLine();
            }
            if (groupFirst < _errors.Count)
            {
                sWrtr.Write("// ");
                Spaces(sWrtr, maxEpilogWidth - 3);
                sWrtr.WriteLine();
            }
            //
            //  And dump the tail of the file
            //
            nxtC = buff.Read();
            while (nxtC != ScanBuff.EndOfFile)
            {
                sWrtr.Write((char)nxtC);
                nxtC = buff.Read();
            }
            ListDivider(sWrtr); sWrtr.WriteLine();
            sWrtr.Flush();
            // sWrtr.Close();
        }

        internal static void ListDivider(StreamWriter wtr)
        {
            wtr.WriteLine(
            "// =========================================================================="
            );
        }

        internal static void Spaces(StreamWriter wtr, int len)
        {
            for (int i = 0; i < len; i++) wtr.Write('-');
        }


        // -----------------------------------------------------
        //   Console Error Reporting Method
        // -----------------------------------------------------

        internal void DumpAll(ScanBuff buff, TextWriter wrtr) {
            if (buff == null) {
                PanicDump(wrtr); return;
            }
            //
            //  Errors are sorted by line number
            //
            _errors = SortedErrorList();
            //
            int  line = 1;
            int  eNum = 0;
            int  eLin = 0;
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
            for (eNum = 0; eNum < _errors.Count; eNum++) {
                eLin = _errors[eNum].Span.startLine;
                if (eLin > currentLine) {
                    //
                    // Spill all the waiting messages
                    //
                    if (currentCol > 0) {
                        wrtr.WriteLine();
                        currentCol = 0;
                    }
                    for (int i = groupFirst; i < eNum; i++) {
                        Error err = _errors[i];
                        wrtr.Write((err.IsWarning ? "Warning: " : "Error: "));
                        wrtr.Write(err.Message);    
                        wrtr.WriteLine();    
                    }
                    currentLine = eLin;
                    groupFirst  = eNum;
                } 
                //
                //  Skip lines up to *but not including* the error line
                //
                while (line < eLin) {
                    nxtC = buff.Read();
                    if (nxtC == (int)'\n') line++;
                    else if (nxtC == ScanBuff.EndOfFile) break;
                } 
                //
                //  Emit the error line
                //
                if (line <= eLin) {
                    wrtr.Write((char)((eLin/1000)%10+(int)'0'));
                    wrtr.Write((char)((eLin/100)%10+(int)'0'));
                    wrtr.Write((char)((eLin/10)%10+(int)'0'));
                    wrtr.Write((char)((eLin)%10+(int)'0'));
                    wrtr.Write(' ');
                    while (line <= eLin) {
                        nxtC = buff.Read();
                        if (nxtC == (int)'\n') line++;
                        else if (nxtC == ScanBuff.EndOfFile) break;
                        wrtr.Write((char)nxtC);
                    } 
                } 
                //
                //  Now emit the error message(s)
                //
                if (_errors[eNum].Span.startColumn >= 0 && _errors[eNum].Span.startColumn < 75) {
                    if (currentCol == 0) {
                        wrtr.Write("-----");
                    }
                    for (int i = currentCol; i < _errors[eNum].Span.startColumn - 1; i++, currentCol++) {
                        wrtr.Write('-');
                    } 
                    for ( ; currentCol < _errors[eNum].Span.endColumn && currentCol < 75; currentCol++)
                        wrtr.Write('^');
                }
            }
            //
            //  Clean up after last message listing
            //  Spill all the waiting messages
            //
            if (currentCol > 0) {
                wrtr.WriteLine();
            }
            for (int i = groupFirst; i < _errors.Count; i++) {
                Error err = _errors[i];
                wrtr.Write((err.IsWarning ? "Warning: " : "Error: "));
                wrtr.Write(err.Message);    
                wrtr.WriteLine();    
            }
        }

        private void PanicDump(TextWriter wrtr) {
            foreach (Error err in _errors) {
                wrtr.Write((err.IsWarning ? "Warning: " : "Error: "));
                wrtr.Write(err.Message);
                wrtr.WriteLine();
            }

        }
    }
}