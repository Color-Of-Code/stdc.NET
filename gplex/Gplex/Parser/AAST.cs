// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using QUT.Gplex;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// This class represents the Attributed Abstract Syntax Tree
    /// corresponding to an input LEX file.
    /// </summary>
    public sealed partial class AAST : IAbstractSyntaxTree
    {
        internal Lexer.Scanner scanner;
        internal ErrorHandler hdlr;

        internal IList<ISpan> usingStrs = new List<ISpan>();  // "using" dotted names
        internal LexSpan nameString;                             // Namespace dotted name

        internal string visibility = "public";                   // Visibility name "public" or "internal"
        internal string scanBaseName = "ScanBase";               // Name of scan base class.
        internal string tokenTypeName = "Tokens";                // Name of the token enumeration type.
        internal string scannerTypeName = "Scanner";             // Name of the generated scanner class.

        internal IList<RuleDesc> ruleList = new List<RuleDesc>();
        internal IList<LexCategory> lexCatsWithPredicates = new List<LexCategory>();
        internal IDictionary<string, LexCategory> lexCategories = new Dictionary<string, LexCategory>();
        internal IDictionary<string, PredicateLeaf> cats; // Allocated on demand
        internal IDictionary<string, StartState> startStates = new Dictionary<string, StartState>();
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
            Prolog = new List<ISpan>();
            Epilog = new List<ISpan>();
            CodeIncl = new List<ISpan>();
        }

        // Text from the user code section
        internal LexSpan UserCode { get; set; }

        // Text to copy verbatim into output file
        internal IList<ISpan> CodeIncl { get; private set; }
        
        // Verbatim declarations for scanning routine
        internal IList<ISpan> Prolog { get; private set; }
        
        // Epilog code for the scanning routine
        internal IList<ISpan> Epilog { get; private set; }

        internal void AddCodeSpan(Destination dest, ISpan span)
        {
            if (!span.IsInitialized) return;
            switch (dest)
            {
                case Destination.codeIncl: CodeIncl.Add(span); break;
                case Destination.scanProlog: Prolog.Add(span); break;
                case Destination.scanEpilog: Epilog.Add(span); break;
            }
        }

        internal void AddVisibility(ISpan span)
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


        internal bool AddLexCategory(string name, string verb, ISpan spn)
        {
            if (lexCategories.ContainsKey(name))
                return false;

            var cls = new LexCategory(name, verb, spn);
            lexCategories.Add(name, cls);
            cls.ParseRegularExpression(this);
            return true;
        }

        internal void AddLexCatPredicate(string name, ISpan span)
        {
            LexCategory cat;
            if (!lexCategories.TryGetValue(name, out cat))
                hdlr.ListError(span, 55, name);
            else if (cat.RegularExpressionTree.Operator != RegOp.CharacterClass)
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

            ISpan lastSpan = Parser.BlankSpan;
            foreach (var rule in ruleList.Reverse())
            {
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
        internal void InitCharacterPredicates()
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
                InitCharacterPredicates();
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

#region Regular Expression Parser class
        #endregion // Regular Expression Parser
    }
}
