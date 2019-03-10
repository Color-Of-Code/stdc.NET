// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// This class represents the Attributed Abstract Syntax Tree
    /// corresponding to an input LEX file.
    /// </summary>
    internal sealed partial class AAST
    {
        internal QUT.Gplex.Lexer.Scanner scanner;
        internal ErrorHandler hdlr;

        internal IList<LexSpan> usingStrs = new List<LexSpan>();  // "using" dotted names
        internal LexSpan nameString;                             // Namespace dotted name

        internal string visibility = "public";                   // Visibility name "public" or "internal"
        internal string scanBaseName = "ScanBase";               // Name of scan base class.
        internal string tokenTypeName = "Tokens";                // Name of the token enumeration type.
        internal string scannerTypeName = "Scanner";             // Name of the generated scanner class.

        internal IList<RuleDesc> ruleList = new List<RuleDesc>();
        internal IList<LexCategory> lexCatsWithPredicates = new List<LexCategory>();
        internal IDictionary<string, LexCategory> lexCategories = new Dictionary<string, LexCategory>();
        internal Dictionary<string, PredicateLeaf> cats; // Allocated on demand
        internal Dictionary<string, StartState> startStates = new Dictionary<string, StartState>();
        private List<StartState> inclStates = new List<StartState>();

        internal Automaton.TaskState Task { get; private set; }
        internal int CodePage { get { return Task.CodePage; } }
        internal bool IsVerbose { get { return Task.Verbose; } }
        internal bool HasPredicates { get { return lexCatsWithPredicates.Any(); } }

        internal AAST(Automaton.TaskState t)
        {
            Task = t;
            startStates.Add(StartState.initState.Name, StartState.initState);
            startStates.Add(StartState.allState.Name, StartState.allState);
            Prolog = new List<LexSpan>();
            Epilog = new List<LexSpan>();
            CodeIncl = new List<LexSpan>();
        }

        // Text from the user code section
        internal LexSpan UserCode { get; set; }

        // Text to copy verbatim into output file
        internal IList<LexSpan> CodeIncl { get; private set; }
        
        // Verbatim declarations for scanning routine
        internal IList<LexSpan> Prolog { get; private set; }
        
        // Epilog code for the scanning routine
        internal IList<LexSpan> Epilog { get; private set; }

        internal void AddCodeSpan(Destination dest, LexSpan span)
        {
            if (!span.IsInitialized) return;
            switch (dest)
            {
                case Destination.codeIncl: CodeIncl.Add(span); break;
                case Destination.scanProlog: Prolog.Add(span); break;
                case Destination.scanEpilog: Epilog.Add(span); break;
            }
        }

        internal void AddVisibility(LexSpan span)
        {
            string result = span.ToString();
            if (result.Equals("internal") || result.Equals("public"))
                visibility = result;
            else
                hdlr.ListError(span, 98);
        }

        internal void SetScanBaseName(string name)
        {
            scanBaseName = name;
        }

        internal void SetTokenTypeName(string name)
        {
            tokenTypeName = name;
        }

        internal void SetScannerTypeName(string name)
        {
            scannerTypeName = name;
        }


        internal bool AddLexCategory(string name, string verb, LexSpan spn)
        {
            if (lexCategories.ContainsKey(name))
                return false;
            else
            {
                var cls = new LexCategory(name, verb, spn);
                lexCategories.Add(name, cls);
                cls.ParseRE(this);
                return true;
            }
        }

        internal void AddLexCatPredicate(string name, LexSpan span)
        {
            LexCategory cat;
            if (!lexCategories.TryGetValue(name, out cat))
                hdlr.ListError(span, 55, name);
            else if (cat.regX.op != RegOp.charClass)
                hdlr.ListError(span, 71, name);
            else if (!cat.HasPredicate)
            {
                cat.HasPredicate = true;
                lexCatsWithPredicates.Add(cat);
                // Add a dummy exclusive start state for the predicate
                AddDummyStartState(cat.PredDummyName);
            }
        }

        //internal bool LookupLexCategory(string name)
        //{ return lexCategories.ContainsKey(name); }

        internal bool AddStartState(bool isX, string name)
        {
            return AddStartState(isX, false, name);
        }

        internal void AddDummyStartState(string name)
        {
            AddStartState(true, true, name);
        }

        bool AddStartState(bool isX, bool isDummy, string name)
        {
            if (name != null)
                if (startStates.ContainsKey(name))
                    return false;
                else
                {
                    var state = new StartState(isDummy, name);
                    startStates.Add(name, state);
                    if (!isX)
                        inclStates.Add(state);
                }
            return true;
        }

        internal StartState StartStateValue(string name)
        {
            StartState state;
            return (startStates.TryGetValue(name, out state) ? state : null);
        }

        internal int StartStateCount { get { return startStates.Count; } }

        internal void AddToAllStates(RuleDesc rule)
        {
            foreach (var p in startStates)
            {
                StartState s = p.Value;
                if (!s.IsAll && !s.IsDummy)
                    s.AddRule(rule);
            }
        }

        internal void FixupBarActions()
        {
            foreach (var cat in this.lexCatsWithPredicates)
                ruleList.Add(RuleDesc.MkDummyRuleDesc(cat, this));

            LexSpan lastSpan = Parser.BlankSpan;
            for (int i = ruleList.Count - 1; i >= 0; i--)
            {
                RuleDesc rule = ruleList[i];
                if (!rule.isBarAction) lastSpan = rule.aSpan;
                else if (!lastSpan.IsInitialized)
                    hdlr.ListError(rule.pSpan, 59);
                else rule.aSpan = lastSpan;
                AddRuleToList(rule);
                // Now give the warning for
                // patterns that consume no input text.
                if (rule.IsLoopRisk)
                    hdlr.ListError(rule.pSpan, 115);
            }
        }

        /// <summary>
        /// This method lazily constructs the dictionary for the
        /// character predicates.  Beware however, that this just
        /// maps the first "crd" characters of the unicode value set.
        /// </summary>
        private void InitCharCats()
        {
            cats = new Dictionary<string, PredicateLeaf>();
            cats.Add("IsControl", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsControl, Char.IsControl)));
            cats.Add("IsDigit", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsDigit, Char.IsDigit)));
            cats.Add("IsLetter", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsLetter, Char.IsLetter)));
            cats.Add("IsLetterOrDigit", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsLetterOrDigit, Char.IsLetterOrDigit)));
            cats.Add("IsLower", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsLower, Char.IsLower)));
            cats.Add("IsNumber", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsNumber, Char.IsNumber)));
            cats.Add("IsPunctuation", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsPunctuation, Char.IsPunctuation)));
            cats.Add("IsSeparator", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsSeparator, Char.IsSeparator)));
            cats.Add("IsSymbol", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsSymbol, Char.IsSymbol)));
            cats.Add("IsUpper", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsUpper, Char.IsUpper)));
            cats.Add("IsWhiteSpace", new PredicateLeaf(PredicateLeaf.MkCharTest(Char.IsWhiteSpace, Char.IsWhiteSpace)));
            cats.Add("IsFormatCharacter", new PredicateLeaf(PredicateLeaf.MkCharTest(CharCategory.IsFormat, CharCategory.IsFormat)));
            cats.Add("IdentifierStartCharacter", new PredicateLeaf(PredicateLeaf.MkCharTest(CharCategory.IsIdStart, CharCategory.IsIdStart)));
            cats.Add("IdentifierPartCharacter", new PredicateLeaf(PredicateLeaf.MkCharTest(CharCategory.IsIdPart, CharCategory.IsIdPart)));
            // IdentifierPartCharacters actually include the Format category
            // as well, but are kept separate here so we may attach a different
            // semantic action to identifiers that require canonicalization by
            // the elision of format characters, or the expansion of escapes.
        }


        private void AddUserPredicate(string name, CharTest test)
        {
            if (this.cats == null)
                InitCharCats();
            cats.Add(name, new PredicateLeaf(test));
        }

        /// <summary>
        /// Add a user-specified character predicate to the 
        /// dictionary. The predicate is in some known assembly
        /// accessible to gplex.
        /// </summary>
        /// <param name="name">the gplex name of the predicate</param>
        /// <param name="aSpan">the simple filename of the assembly</param>
        /// <param name="mSpan">the qualified name of the method</param>
        internal void AddUserPredicate(
            string name,
            LexSpan aSpan,
            LexSpan mSpan)
        {
            // maybe we need (1) type dotted name, (2) method name only?
            string mthName = mSpan.ToString();
            string asmName = aSpan.ToString();
            int offset = mthName.LastIndexOf('.');
            string clsName = mthName.Substring(0, offset);
            string mthIdnt = mthName.Substring(offset + 1);

            try
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFrom(asmName);
                System.Type[] types = asm.GetExportedTypes();
                foreach (Type type in types)
                {
                    if (type.FullName.Equals(clsName, StringComparison.OrdinalIgnoreCase) ||
                        type.Name.Equals(clsName, StringComparison.OrdinalIgnoreCase))
                    {
                        QUT.Gplex.ICharTestFactory factory =
                            (ICharTestFactory)System.Activator.CreateInstance(type);

                        if (factory != null)
                        {
                            CharTest test = factory.GetDelegate(mthIdnt);
                            if (test == null)
                                hdlr.ListError(mSpan, 97, mthIdnt);
                            else
                                AddUserPredicate(name, test);
                            return;
                        }
                    }
                }
                // Class not found error not reported until ALL
                // classes exported from the assembly have been checked.
                hdlr.ListError(mSpan, 96, clsName); return;
            }
            catch (FileNotFoundException) { hdlr.ListError(aSpan, 94); }
            catch (FileLoadException) { hdlr.ListError(aSpan, 95); }
            catch (Exception x) { hdlr.AddError(x.Message, aSpan.Merge(mSpan)); throw; }
        }

        internal void AddRuleToList(RuleDesc rule)
        {
            //
            // Versions before 0.4.2.* had incorrect semantics
            // for the handling of inclusive start states.
            // Correct semantics for inclusive start states:
            // If a rule has no explicit start state(s) then it
            // should be added to *every* inclusive start state.
            //
            // For version 0.5.1+ the semantics follow those of
            // FLEX, which distinguishes between rules that are
            // *explicitly* attached to INITIAL, and those which
            // have an empty start state list.  Only those without
            // a start state list are added to inclusive states.
            //
            if (rule.list == null || rule.list.Count == 0)
            {
                StartState.initState.AddRule(rule);       // Add to initial state
                foreach (var inclS in inclStates)  // Add to inclusive states
                    inclS.AddRule(rule);
            }
            else if (rule.list[0].IsAll)
                AddToAllStates(rule);
            else
                foreach (var state in rule.list)
                    state.AddRule(rule);
        }

        internal LexSpan AtStart { get { var tmp = new LexSpan(1, 1, 1, 1, 0, 0, scanner.Buffer); return tmp; } }

        // =============================================================================
        #region Regular Expression Parser class
        /// <summary>
        /// NESTED CLASS This is a hand-written, recursive descent parser.
        /// No error recovery attempted, instead an exception is thrown and
        /// the parse abandoned. The exception catch handler transforms the 
        /// exception into a regular error diagnostic.
        /// </summary>
        internal sealed class ReParser
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
            void InitReParser()
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

            internal ReParser(string str, LexSpan spn, AAST parent)
            {
                var parentTask = parent.Task;
                if (parentTask.UseUnicode)
                    CharacterUtilities.SetUnicode();
                symCard = parentTask.HostSymCardinality;
                pat = str;
                span = spn;
                InitReParser();
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
                    tmp = new Unary(RegOp.leftAnchor, Simple());
                }
                else
                    tmp = Simple();
                if (!esc && chr == '$')
                {
                    scan();
                    tmp = new Unary(RegOp.rightAnchor, tmp);
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
                    return new Binary(RegOp.context, tmp, Term());
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
                    tmp = new Binary(RegOp.alt, tmp, Factor());
                }
                return tmp;
            }

            // Factor : Primary {Primary} ;
            internal RegExTree Factor()
            {
                RegExTree tmp = Primary();
                while (prStart[(int)chr] || esc)
                    tmp = new Binary(RegOp.concat, tmp, Primary());
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
                    tmp = new Unary(RegOp.closure, tmp);
                }
                else if (!esc && chr == '+')
                {
                    pls = new Unary(RegOp.closure, tmp);
                    pls.minRep = 1;
                    scan();
                    tmp = pls;
                }
                else if (!esc && chr == '?')
                {
                    pls = new Unary(RegOp.finiteRep, tmp);
                    pls.minRep = 0;
                    pls.maxRep = 1;
                    scan();
                    tmp = pls;
                }
                else if (!esc && chr == '{' && Char.IsDigit(peek()))
                {
                    pls = new Unary(RegOp.finiteRep, tmp);
                    GetRepetitions(pls);
                    tmp = pls;
                }
                return tmp;
            }

            // Repetitions : "{" IntLiteral ["," [IntLiteral] ] "}" ;
            internal void GetRepetitions(Unary tree)
            {
                scan();          // read past '{'
                tree.minRep = GetInt();
                if (!esc && chr == ',')
                {
                    scan();
                    if (Char.IsDigit(chr))
                        tree.maxRep = GetInt();
                    else
                        tree.op = RegOp.closure;
                }
                else
                    tree.maxRep = tree.minRep;
                checkAndScan('}');
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
                    var leaf = new Leaf(RegOp.charClass);
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
                    Leaf leaf = cat.regX as Leaf;
                    if (leaf != null && leaf.op == RegOp.charClass)
                        leaf.rangeLit.name = name;
                    return cat.regX;
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
                var leaf = new Leaf(RegOp.charClass);
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
                    parent.InitCharCats();
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
                    if (namedRE.regX.op != RegOp.charClass)
                        Error(71, start, name.Length, name);
                    return namedRE.regX as Leaf;
                }
                //
                // else name not known
                //
                Error(76, start, name.Length, name);
                return null;
            }
        }
        #endregion // Regular Expression Parser
        // =============================================================================
    } // end AAST class.

}
