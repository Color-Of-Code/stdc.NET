// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System;
using System.Collections;
using System.Globalization;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    // =============================================================================
    #region Regular Expression Parser class
    /// <summary>
    /// NESTED CLASS This is a hand-written, recursive descent parser.
    /// No error recovery attempted, instead an exception is thrown and
    /// the parse abandoned. The exception catch handler transforms the 
    /// exception into a regular error diagnostic.
    /// </summary>
    internal sealed class RegularExpressionParser
    {

        // ============================================================
        // Here is the EBNF grammar for regular expressions.
        // Note carefully that this grammar does not use the same
        // metalanguage as GPPG specifications. In particular '['elem']' 
        // denotes an optional element, and '{'elem'}' denotes (Kleene) 
        // closure, with '{'elem'}+' denotes non-zero repetitions.
        //
        // The actions of the parser build an abstract syntax tree  
        //  with AST node base-type QUT.Gplex.Parser.RegExTree.
        // ============================================================
        // RegEx : "<<EOF>>" | Expr ;
        // Expr : ["^"] SimpleExpr ["$"] ;
        // SimpleExpr : Term ["/" Term] ;
        // Term : Factor {"|" Factor} ;
        // Factor : Primary {Primary} ;
        // Primary : (LitString | "(" Term ")" | Primitive) ["*" | "+" | Repetitions] ;
        // Repetitions : "{" IntLiteral ["," [IntLiteral] ] "}" ;
        // Primitive : CharClassExpr | NamedRegexReference | "." | escapedChar | char ;
        // NamedRegexReference : "{" identifier "}" ;
        // CharClassExpr : CharClass { ("{-}" | "{+}) CharClass } * ;
        // CharClass : "[" ["^"] {code | code "-" code | FilteredClass}+ "]" ;
        // FilteredClass : "[:" predicateName ":]" 
        //               | "[:" CharClassReference ":]"
        //               ;
        // ============================================================

        BitArray prStart;

        const char NUL = '\0';
        int symCard;
        int index;              // index of the *next* character to be read
        char chr;               // the last char to be read.
        bool esc;               // the last character was backslash-escaped
        AAST parent;
        LexSpan span;
        string pat;

        /// <summary>
        /// Defines the character set special to regular expressions
        /// and the valid characters to start syntatic category "Primary"
        /// </summary>
        /// <param name="crd">host alphabet cardinality</param>
        void InitRegularExpressionParser()
        {
            prStart = new BitArray(symCard, true);
            prStart[(int)')'] = false;
            prStart[(int)'|'] = false;
            prStart[(int)'*'] = false;
            prStart[(int)'+'] = false;
            prStart[(int)'?'] = false;
            prStart[(int)'}'] = false;
            prStart[(int)'/'] = false;
            prStart[(int)')'] = false;
            prStart[(int)'$'] = false;
            prStart[(int)'/'] = false;
            prStart[(int)'\0'] = false;
        }

        internal RegularExpressionParser(string str, LexSpan spn, AAST parent)
        {
            var parentTask = parent.Task;
            if (parentTask.UseUnicode)
                CharacterUtilities.SetUnicode();
            symCard = parentTask.HostSymCardinality;
            pat = str;
            span = spn;
            InitRegularExpressionParser();
            this.parent = parent;
            //
            //  This is ugly, but we cannot manipulate
            //  RangeLists unless the alphabet upper bound
            //  is known to the code of class Partition.
            //
            CharRange.Init(parentTask.TargetSymCardinality);
        }


        internal RegExTree Parse()
        {
            try
            {
                RegExTree tmp;
                scan();
                tmp = RegEx();
                return tmp;
            }
            catch (RegExException x)
            {
                x.ListError(parent.hdlr, this.span);
                return null;
            }
            catch (StringInterpretException x)
            {
                parent.hdlr.ListError(this.span, 99, x.Key);
                return null;
            }
        }

        internal void scan()
        {
            int len = pat.Length;
            chr = (index == len ? NUL : pat[index++]);
            esc = (chr == '\\');
            if (esc)
                chr = (index == len ? NUL : pat[index++]);
        }

        /// <summary>
        /// Do lookahead one position in string buffer
        /// </summary>
        /// <returns>lookahead character or NUL if at end of string</returns>
        internal char peek()
        {
            return (index == pat.Length ? NUL : pat[index]);
        }

        internal char peek2()
        {
            return (index + 1 == pat.Length ? NUL : pat[index + 1]);
        }

        internal bool isEofString()
        {
            // The EOF string must be exactly "<<EOF>>"
            return (pat.Length >= 7 && pat[0] == '<' && pat.Substring(0, 7).Equals("<<EOF>>"));
        }

        internal int GetInt()
        {
            int val = (int)chr - (int)'0';
            scan();
            while (Char.IsDigit(chr))
            {
                checked { val = val * 10 + (int)chr - (int)'0'; }
                scan();
            }
            return val;
        }

        static void Error(int num, int idx, int len, string str)
        {
            throw new RegExException(num, idx, len, str);
        }

        void Warn(int num, int idx, int len, string str)
        {
            parent.hdlr.ListError(span.FirstLineSubSpan(idx, len), num, str, '"');
        }

        void Warn(int num, int idx, int len)
        {
            parent.hdlr.ListError(span.FirstLineSubSpan(idx, len), num);
        }

        internal void checkAndScan(char ex)
        {
            if (chr == ex)
                scan();
            else
                Error(53, index - 1, 1, "'" + ex + "'");
        }

        internal void checkAndScan(CharPredicate p, string predName)
        {
            if (p(chr))
                scan();
            else
                Error(103, index - 1, 1, predName);
        }

        // RegEx : "<<EOF>>" | Expr ;
        internal RegExTree RegEx()
        {
            if (isEofString())
                return new Leaf(RegOp.eof);
            else
            {
                RegExTree tmp = Expr();
                if (chr != '\0')
                    Error(101, index - 1, pat.Length - index + 1, null);
                return tmp;
            }
        }

        // Expr : ["^"] SimpleExpr ["$"] ;
        internal RegExTree Expr()
        {
            RegExTree tmp;
            if (!esc && chr == '^')
            {
                scan();
                tmp = new Unary(RegOp.LeftAnchor, Simple());
            }
            else
                tmp = Simple();
            if (!esc && chr == '$')
            {
                scan();
                tmp = new Unary(RegOp.RightAnchor, tmp);
            }
            return tmp;
        }

        // SimpleExpr : Term ["/" Term] ;
        internal RegExTree Simple()
        {
            RegExTree tmp = Term();
            if (!esc && chr == '/')
            {
                scan();
                return new Binary(RegOp.Context, tmp, Term());
            }
            return tmp;
        }

        // Term : Factor {"|" Factor} ;
        internal RegExTree Term()
        {
            RegExTree tmp = Factor();
            while (!esc && chr == '|')
            {
                scan();
                tmp = new Binary(RegOp.Alternation, tmp, Factor());
            }
            return tmp;
        }

        // Factor : Primary {Primary} ;
        internal RegExTree Factor()
        {
            RegExTree tmp = Primary();
            while (prStart[(int)chr] || esc)
                tmp = new Binary(RegOp.Concatenation, tmp, Primary());
            return tmp;
        }


        internal RegExTree LitString()
        {
            int pos = index;
            int len;
            string str;
            scan();                 // get past '"'
            while (esc || (chr != '"' && chr != NUL))
                scan();
            len = index - 1 - pos;
            checkAndScan('"');
            str = pat.Substring(pos, len);
            try
            {
                str = CharacterUtilities.InterpretCharacterEscapes(str);
            }
            catch (RegExException x)
            {
                // InterpretCharacterEscapes takes only a
                // substring of "this.pat". RegExExceptions
                // that are thrown will have an index value
                // relative to this substring, so the index
                // is transformed relative to "this.pat".
                x.AdjustIndex(pos);
                throw;
            }
            return new Leaf(str);
        }

        // Primary : (LitString | "(" Term ")" | Primitive) ["*" | "+" | Repetitions] ;
        internal RegExTree Primary()
        {
            RegExTree tmp;
            Unary pls;
            if (!esc && chr == '"')
                tmp = LitString();
            else if (!esc && chr == '(')
            {
                scan();
                tmp = Term();
                checkAndScan(')');
            }
            else
                tmp = Primitive();

            if (!esc && chr == '*')
            {
                scan();
                tmp = new Unary(RegOp.Closure, tmp);
            }
            else if (!esc && chr == '+')
            {
                pls = new Unary(RegOp.Closure, tmp, 1);
                scan();
                tmp = pls;
            }
            else if (!esc && chr == '?')
            {
                pls = new Unary(RegOp.FiniteRepetition, tmp, 0, 1);
                scan();
                tmp = pls;
            }
            else if (!esc && chr == '{' && Char.IsDigit(peek()))
            {
                // Repetitions : "{" IntLiteral ["," [IntLiteral] ] "}" ;
                scan();          // read past '{'
                int minimumOfRepetitions = GetInt();
                int maximumOfRepetitions = 0;
                RegOp op = RegOp.FiniteRepetition;
                if (!esc && chr == ',')
                {
                    scan();
                    if (Char.IsDigit(chr))
                        maximumOfRepetitions = GetInt();
                    else
                        op = RegOp.Closure;
                }
                else
                    maximumOfRepetitions = minimumOfRepetitions;
                pls = new Unary(op, tmp, minimumOfRepetitions, maximumOfRepetitions);
                checkAndScan('}');
                tmp = pls;
            }
            return tmp;
        }

        int EscapedChar()
        {
            index--;
            return CharacterUtilities.EscapedChar(pat, ref index);
        }

        int CodePoint()
        {
            if (!Char.IsHighSurrogate(chr))
                return (int)chr;
            index--;
            return CharacterUtilities.CodePoint(pat, ref index);
        }

        // Primitive : CharClassExpr | NamedRegexReference | "." | escapedChar | char ;
        internal RegExTree Primitive()
        {
            RegExTree tmp;
            if (!esc && chr == '[')
                tmp = CharClassExpr();
            else if (!esc && chr == '{' && !Char.IsDigit(peek()))
                tmp = UseRegexRef();
            else if (!esc && chr == '.')
            {
                var leaf = new Leaf(RegOp.CharacterClass);
                leaf.rangeLit = new RangeLiteral(true);
                scan();
                leaf.rangeLit.list.Add(new CharRange('\n'));
                tmp = leaf;
            }
            // Remaining cases are:
            //  1. escaped character (maybe beyond ffff limit)
            //  2. ordinary unicode character
            //  3. maybe a surrogate pair in future
            else if (esc)
            {
                tmp = new Leaf(EscapedChar());
                scan();
            }
            else
            {
                tmp = new Leaf((int)chr);
                scan();
            }
            return tmp;
        }

        // NamedRegexReference : "{" identifier "}" ;
        // must be a gplex identifier => ascii names only
        internal RegExTree UseRegexRef()
        {
            // Assert chr == '{'
            int start;
            string name;
            LexCategory cat;
            scan();                                     // read past '{'
            start = index - 1;
            //
            //  The lexical grammar for "pattern" only
            //  allows ident '}' as continuation here.
            //
            while (chr != '}' && chr != '\0')
                scan();
            name = pat.Substring(start, index - start - 1);
            checkAndScan('}');
            if (parent.lexCategories.TryGetValue(name, out cat))
            {
                Leaf leaf = cat.RegularExpressionTree as Leaf;
                if (leaf != null && leaf.Operator == RegOp.CharacterClass)
                    leaf.rangeLit.name = name;
                return cat.RegularExpressionTree;
            }
            else
                Error(55, start, name.Length, name);
            return null;
        }

        // CharClassExpr : CharClass { ("{-}" | "{+}" | "{*}") CharClass } ;
        internal RegExTree CharClassExpr()
        {
            int startIx = index - 1;
            char lookahead;
            Leaf leaf = CharClass();
            while (!esc
                    && chr == '{'
                    && ((lookahead = peek()) == '-' || lookahead == '+' || lookahead == '*')
                    && peek2() == '}')
            {
                scan(); scan(); scan(); // Read past "{x}"
                Leaf rhs = CharClass();
                if (lookahead == '+')
                    leaf.Merge(rhs);
                else if (lookahead == '-')
                    leaf.Subtract(rhs);
                else
                    leaf.Intersect(rhs);
                leaf.rangeLit.list.Canonicalize();
                if (leaf.rangeLit.list.Ranges.Count == 0)
                    Warn(118, startIx, index - startIx);
#if RANGELIST_DIAGNOSTICS
                else
                    Warn( 121, startIx, index - startIx, leaf.rangeLit.list.ToString() );
#endif
            }
            return leaf;
        }

        // CharClass : "[" ["^"] {code | code "-" code | FilteredClass}+ "]" ;
        internal Leaf CharClass()
        {
            // Assert chr == '['
            // Need to build a new string taking into account char escapes
            var leaf = new Leaf(RegOp.CharacterClass);
            bool invert = false;
            scan();                           // read past '['
            if (!esc && chr == '^')
            {
                invert = true;
                scan();                       // read past '^'
            }
            leaf.rangeLit = new RangeLiteral(invert);
            // Special case of '-' at start, taken as ordinary class member.
            // This is correct for LEX specification, but is undocumented
            // behavior for FLEX. GPLEX gives a friendly warning, just in
            // case this is actually a typographical error.
            if (!esc && chr == '-')
            {
                Warn(113, index - 1, 1, "-");
                leaf.rangeLit.list.Add(new CharRange('-'));
                scan();                       // read past -'
            }

            while (chr != NUL && (esc || chr != ']'))
            {
                int lhCodePoint;
                int startIx = index - 1; // save starting index for error reporting
                lhCodePoint = (esc ? EscapedChar() : CodePoint());
                if (!esc && lhCodePoint == (int)'-')
                    Error(82, startIx, index - startIx, null);
                //
                // There are three possible elements here:
                //  * a singleton character
                //  * a character range
                //  * a filtered class like [:IsLetter:]
                //
                if (chr == '[' && !esc && peek() == ':') // character category
                {
                    Leaf rslt = FilteredClass();
                    leaf.Merge(rslt);
                }
                else
                {
                    scan();
                    if (!esc && chr == '-')             // character range
                    {
                        scan();
                        if (!esc && chr == ']')
                        {
                            // Special case of '-' at end, taken as ordinary class member.
                            // This is correct for LEX specification, but is undocumented
                            // behavior for FLEX. GPLEX gives a friendly warning, just in
                            // case this is actually a typographical error.
                            leaf.rangeLit.list.Add(new CharRange(lhCodePoint));
                            leaf.rangeLit.list.Add(new CharRange('-'));
                            //Error(81, idx, index - idx - 1);
                            Warn(114, startIx, index - startIx - 1, String.Format(
                                CultureInfo.InvariantCulture,
                                "'{0}','{1}'",
                                CharacterUtilities.Map(lhCodePoint),
                                '-'));
                        }
                        else
                        {
                            int rhCodePoint = (esc ? EscapedChar() : CodePoint());
                            if (rhCodePoint < lhCodePoint)
                                Error(54, startIx, index - startIx, null);
                            scan();
                            leaf.rangeLit.list.Add(new CharRange(lhCodePoint, rhCodePoint));
                        }
                    }
                    else                               // character singleton
                    {
                        leaf.rangeLit.list.Add(new CharRange(lhCodePoint));
                    }
                }
            }
            checkAndScan(']');
            leaf.rangeLit.list.Canonicalize();
            return leaf;
        }

        // PredicatedClass : "[:" predicateName ":]" ;
        // predicate name could be any C# identifier.
        private Leaf FilteredClass()
        {
            // Assert: chr == '[', next is ':'
            int start;
            string name;
            Leaf rslt;
            scan(); // read past '['
            scan(); // read past ':'
            start = index - 1;
            //
            // Get the C# identifier name.
            //
            checkAndScan(CharCategory.IsIdStart, "IdStart");
            while (CharCategory.IsIdPart(chr))
                scan();
            name = pat.Substring(start, index - start - 1);
            rslt = GetPredicate(name, start);
            checkAndScan(':');
            checkAndScan(']');
            return rslt;
        }

        private Leaf GetPredicate(string name, int start)
        {
            // lazy allocation of dictionary
            if (parent.cats == null)
                parent.InitCharacterPredicates();
            //
            // Try to find name in "cats" list
            //
            PredicateLeaf rslt;
            bool found = parent.cats.TryGetValue(name, out rslt);
            // lazily populate element range lists
            if (found)
            {
                if (rslt.rangeLit == null)
                    rslt.Populate(name, parent);
                return rslt;
            }
            //
            // else see if name is named regex reference
            //
            LexCategory namedRE;
            found = parent.lexCategories.TryGetValue(name, out namedRE);
            if (found)
            {
                if (namedRE.RegularExpressionTree.Operator != RegOp.CharacterClass)
                    Error(71, start, name.Length, name);
                return namedRE.RegularExpressionTree as Leaf;
            }
            //
            // else name not known
            //
            Error(76, start, name.Length, name);
            return null;
        }
    }
    #endregion // Regular Expression Parser
}
