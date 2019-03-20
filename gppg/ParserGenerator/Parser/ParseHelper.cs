// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough QUT 2010
// (see accompanying GPPGcopyright.rtf)

using System;
using System.Globalization;
using System.Collections.Generic;
using QUT.GPGen.Lexers;
using QUT.Gplib;

namespace QUT.GPGen.Parser
{
    internal partial class Parser
    {
        internal ErrorHandler handler;

        private Grammar grammar = new Grammar();
        internal Grammar Grammar { get { return grammar; } }

        private string baseName;
        private string sourceFileInfo;

        internal string ListfileName { get { return baseName + ".lst"; } }
        internal string SourceFileInfo { get { return sourceFileInfo; } }

        private NonTerminal currentLHS;

        enum TokenProperty { token, left, right, nonassoc }

        internal Parser(string filename, string fileinfo, Scanner scanner, ErrorHandler handler)
            : base(scanner)
        {
            this.handler = handler;
            this.sourceFileInfo = fileinfo;
            this.baseName = System.IO.Path.GetFileNameWithoutExtension(filename);
            grammar.InputFileIdent = fileinfo;
            grammar.InputFilename = filename;
        }

        // ===============================================================
        //
        //  Various helpers for the semantic actions of the parser
        //  Definition Part Helpers
        //
        // ===============================================================

        internal void SetSemanticType(ISpan span)
        {
            if (grammar.ValueTypeNameSpan != null)
            {
                handler.ListError(grammar.ValueTypeNameSpan, 72);
                handler.ListError(span, 72);
            }
            else
            {
                grammar.ValueTypeNameSpan = span;
                grammar.ValueTypeName = span.ToString();
            }
        }



        private void DeclareTokens(PrecedenceType prop, string kind, IEnumerable<IToken> list)
        {
            grammar.BumpPrec();
            foreach (IToken info in list) {
                Token token = (IsLitChar(info.Name) ? Token.litchar : Token.ident);
                Terminal t = grammar.LookupOrDefineTerminal(token, info.Name, info.Alias);
                if (prop != PrecedenceType.token)
                    t.prec = new Precedence(prop, grammar.Prec);
                if (!String.IsNullOrEmpty(kind))
                    t.Kind = kind;
            }
        }

        internal string GetLitString(ISpan span)
        {
            string text = span.ToString();
            if (text[0] != '\"' || text[text.Length - 1] != '\"')
                throw new ToolInternalException("Internal error: invalid litstring");
            text = text.Substring(1, text.Length - 2);
            try
            {
                text = CharacterUtilities.InterpretCharacterEscapes(text);
            }
            catch (StringInterpretException e)
            {
                handler.ListError(span, 70, e.Message, '\'');
            }
            return text;
        }

        internal static string GetVerbatimString(ISpan span)
        {
            string text = span.ToString();
            if (text[0] != '@' || text[1] != '\"' || text[text.Length - 1] != '\"')
                throw new ToolInternalException("Internal error: invalid litstring");
            text = text.Substring(2, text.Length - 3);
            return CharacterUtilities.InterpretEscapesInVerbatimString(text);
        }

        private void DeclareNtKind(string kind, List<string> list)
        {
            foreach (string name in list)
            {
                NonTerminal nt = grammar.LookupNonTerminal(name);
                nt.Kind = kind;
            }
        }

        private void TidyUpDefinitions(ISpan def)
        {
            handler.DefaultSpan = def;
            if (GPCG.Defines) grammar.TokFileName = baseName + ".tokens";
            if (GPCG.Conflicts) grammar.DiagFileName = baseName + ".conflicts";
            // If both %union AND %YYSTYPE have been set, YYSTYPE must be
            // a simple name, and not a type-constructor. Check this now!
            if (grammar.unionType != null && 
                grammar.ValueTypeName != null &&
                grammar.ValueTypeName.LastIndexOfAny(new char[] { '.', '[', '<' }) > 0)
            {
                handler.ListError(grammar.ValueTypeNameSpan, 71);
            }
        }

        // ===============================================================
        //
        //  Various helpers for the semantic actions of the parser
        //  Rules Part Helpers
        //
        // ===============================================================

        private void SetCurrentLHS(ISpan lhs) 
        {
            string lhsName = lhs.ToString();
            NonTerminal nt = grammar.LookupNonTerminal(lhsName);
            if (grammar.terminals.ContainsKey(lhsName))
                handler.ListError(lhs, 76);
            currentLHS = nt;
            if (grammar.startSymbol == null)
                grammar.startSymbol = nt;

            if (grammar.productions.Count == 0)
                grammar.CreateSpecialProduction(grammar.startSymbol);
        }

        private void ClearCurrentLHS() { currentLHS = null; }

        private Production NewProduction()
        {
            return new Production(currentLHS);
        }

        private Production NewProduction(IEnumerable<string> symbols, ActionProxy proxy)
        {
            Production rslt = new Production(currentLHS);
            if (symbols != null)
                AddSymbolsToProduction(rslt, symbols);
            if (proxy != null)
                AddActionToProduction(rslt, proxy);
            return rslt;
        }

        private void AddSymbolsToProduction(Production prod, IEnumerable<string> list)
        {
            // Version 1.3.1 sends even empty lists to this method.
            // Furthermore, version 1.3.1 no longer explicitly calls
            // FixInternalReduction().  It is easier to adopt a consistent
            // approach and let AddXxxToProd check for a trailing action
            // prior to adding symbols or a new action.
            //
            if (list != null) 
            {
                if (prod.semanticAction != null || prod.precSpan != null)
                    FixInternalReduction(prod);
                foreach (string str in list)
                {
                    ISymbol symbol = null;
                    switch (TokenOf(str))
                    {
                        case Token.litchar:
                            symbol = grammar.LookupTerminal(Token.litchar, str);
                            break;
                        case Token.litstring:
                            symbol = grammar.aliasTerms[str];
                            break;
                        case Token.ident:
                            if (grammar.terminals.ContainsKey(str))
                                symbol = grammar.terminals[str];
                            else
                                symbol = grammar.LookupNonTerminal(str);
                            break;
                    }
                    prod.rhs.Add(symbol);;
                }
            }
        }

        private void AddActionToProduction(Production prod, ActionProxy proxy)
        {
            // Version 1.3.1 no longer explicitly calls FixInternalReduction().  
            // It is easier to adopt a consistent approach and
            // let AddXxxToProd check for a trailing action
            // prior to adding symbols or a new action.
            //
            if (proxy != null)
            {
                if (prod.semanticAction != null || prod.precSpan != null)
                    FixInternalReduction(prod);
                ISpan cSpan = proxy.codeBlock;      // LexSpan of action code
                ISpan pSpan = proxy.precedenceToken;      // LexSpan of ident in %prec ident
                if (pSpan != null)
                {
                    prod.prec = grammar.LookupTerminal(Token.ident, pSpan.ToString()).prec;
                    prod.precSpan = proxy.precedenceSpan;
                }
                if (cSpan != null)
                {
                    prod.semanticAction = new SemanticAction(prod, prod.rhs.Count, cSpan);
                }
            }
        }

        private void FixInternalReduction(Production prod)
        {
            // This production has an action or precedence. 
            // Before more symbols can be added to the rhs
            // the existing action must be turned into an
            // internal reduction, and the action (and 
            // precedence, if any) moved to the new reduction.
            //
            if (prod.semanticAction != null)
            {
                string anonName = "Anon@" + (++grammar.NumActions).ToString(CultureInfo.InvariantCulture);
                NonTerminal anonNonT = grammar.LookupNonTerminal(anonName);
                Production EmptyProd = new Production(anonNonT);
                EmptyProd.semanticAction = prod.semanticAction;
                EmptyProd.prec = prod.prec;

                grammar.AddProduction(EmptyProd);
                prod.rhs.Add(anonNonT);
            }
            if (prod.precSpan != null)
                handler.ListError(prod.precSpan, 102);
            prod.semanticAction = null;
            prod.precSpan = null;
            prod.prec = null;
        }

        private void FinalizeProduction(Production prod)
        {
            if (prod.semanticAction != null)
                // TODO: replace with ISpan
                ActionScanner.CheckSpan(prod.rhs.Count, (LexSpan)prod.semanticAction.Code, handler);
            grammar.AddProduction(prod);
            Precedence.Calculate(prod);
            prod.precSpan = null;
        }

        // ===============================================================
        // ===============================================================

        private static Token TokenOf(string str)
        {
            if (str[0] == '\'') return Token.litchar;
            else if (str[0] == '\"') return Token.litstring;
            else return Token.ident;
        }

        private static bool IsLitChar(string text)
        {
            return text[0] == '\'' && text[text.Length - 1] == '\'';
        }
    }

}
