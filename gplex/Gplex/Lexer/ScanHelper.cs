// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf.

//  Scan helper for 0.9.2 version of gplex
//  kjg 08 September 2008 2006
//

using QUT.Gplex.Parser;
using System.Text;
using System.Text.RegularExpressions;

namespace QUT.Gplex.Lexer
{
    internal partial class Scanner
    {
        public ErrorHandler yyhdlr;
        private int badCount;

        private static Regex _keywordsRegex = null;

        static Regex BuildKeywordsRegex()
        {
            string[] list = {
                "abstract", "as",
                "base", "bool", "break", "byte",
                "case", "catch", "char", "checked", "class", "const", "continue",
                "decimal", "default", "delegate", "do", "double",
                "else", "enum", "event", "explicit", "extern",
                "false", "finally", "fixed", "float", "for", "foreach",
                "goto",
                "if", "implicit", "in", "int", "interface", "internal", "is",
                "lock", "long",
                "namespace", "new", "null",
                "object", "operator", "out", "override",
                "params", "private", "protected", "public",
                "readonly", "ref", "return",
                "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
                "this", "throw", "true", "try", "typeof",
                "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
                "virtual", "void",
                "while", "where"
            };
            var regexstr = $@"^({string.Join('|', list)})$";
            return new Regex(regexstr, RegexOptions.Compiled);
        }

        static Tokens GetIdToken(string str)
        {
            _keywordsRegex = _keywordsRegex ?? BuildKeywordsRegex();
            return _keywordsRegex.IsMatch(str)
                ? Tokens.csKeyword
                : Tokens.csIdent;
        }

        Tokens GetTagToken(string str)
        {
            switch (str)
            {
                case "%x":
                    yy_push_state(NMLST); return Tokens.exclTag;
                case "%s":
                    yy_push_state(NMLST); return Tokens.inclTag;
                case "%using":
                    yy_push_state(LCODE); return Tokens.usingTag;
                case "%scanbasetype":
                    yy_push_state(LCODE); return Tokens.scanbaseTag;
                case "%tokentype":
                    yy_push_state(LCODE); return Tokens.tokentypeTag;
                case "%scannertype":
                    yy_push_state(LCODE); return Tokens.scannertypeTag;
                case "%namespace":
                    yy_push_state(LCODE); return Tokens.namespaceTag;
                case "%option":
                    yy_push_state(VRBTM); return Tokens.optionTag;
                case "%charSetPredicate":
                case "%charClassPredicate":
                    yy_push_state(NMLST); return Tokens.charSetPredTag;
                case "%userCharPredicate":
                    yy_push_state(LCODE); return Tokens.userCharPredTag;
                case "%visibility":
                    yy_push_state(LCODE); return Tokens.visibilityTag;
                default:
                    Error(77, TokenSpan()); return Tokens.repErr;
            }
        }

        public override void yyerror(string format, params object[] args)
        { if (yyhdlr != null) yyhdlr.ListError(TokenSpan(), 1, format); }

        internal void Error(int n, LexSpan s)
        {
            if (yyhdlr != null)
                yyhdlr.ListError(s, n);
        }

        internal void ResetBadCount() { badCount = 0; }

        internal void Error79(LexSpan s)
        {
            Error(79, s);
            badCount++;
            if (badCount >= 3)
                yy_push_state(SKIP);
        }

        internal LexSpan TokenSpan()
        { return new LexSpan(tokLin, tokCol, tokELin, tokECol, tokPos, tokEPos, buffer); }

        int depth;
        LexSpan comStart;
    }
}
