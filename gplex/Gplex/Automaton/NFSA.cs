// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;
using System.Collections.Generic;
using QUT.Gplex.Parser;

namespace QUT.Gplex.Automaton
{
    partial class NFSA
    {
        TaskState task;

        public NfsaInstance[] nfas;

        /// <summary>
        /// This dictionary provides a mapping from 
        /// (current NFSA state ordinal, symbol ordinal) to the 
        /// next NFSA state ordinal. The implementation (since v1.1.6)
        /// uses a dictionary key with 32 bits of state ordinal and
        /// 32 bits of symbol ordinal. Previous versions used only 16
        /// bits of each, and were erroneous for automata with more
        /// than 64k of NFSA states.
        /// </summary>
        public Dictionary<ulong, NState> next = new Dictionary<ulong, NState>();

        public NFSA(TaskState t) { task = t; }

        /// <summary>
        /// Build the NFSA from the abstract syntax tree.
        /// There is an NfsaInstance for each start state.
        /// Each rule starts with a new nfsa state, which
        /// is the target of a new epsilon transition from
        /// the real start state, nInst.Entry.
        /// </summary>
        /// <param name="ast"></param>
        public void Build(AAST ast)
        {
            int index = 0;
            DateTime time0 = DateTime.Now;
            nfas = new NfsaInstance[ast.StartStateCount];
            foreach (KeyValuePair<string, StartState> p in ast.startStates)
            {
                StartState s = p.Value;
                string name = p.Key;
                if (!s.IsAll)
                {
                    NfsaInstance nInst = new NfsaInstance(s, this);
                    nfas[index++] = nInst;
                    nInst.key = name;

                    // for each pattern do ...
                    foreach (RuleDesc rule in s.rules)
                    {
                        RegExTree tree = rule.Tree;

                        // This test constructs the disjoint automata
                        // that test code points for predicate evaluation.
                        if (rule.isPredDummyRule)
                        {
                            NState entry = nInst.Entry;
                            nInst.MakePath(tree, entry, entry);
                        }
                        else
                        {
                            NState start = nInst.MkState();
                            NState endSt = nInst.MkState();

                            if (tree.op == RegOp.leftAnchor)     // this is a left anchored pattern
                            {
                                nInst.AnchorState.AddEpsTrns(start);
                                tree = ((Unary)tree).kid;
                            }
                            else                                // this is not a left anchored pattern
                                nInst.Entry.AddEpsTrns(start);
                            //
                            // Now check for right anchors, and add states as necessary.
                            //
                            if (tree.op == RegOp.eof)
                            {
                                //
                                // <<EOF>> rules are always emitted outside
                                // of the usual subset construction framework.
                                // We ensure that we do not get spurious warnings.
                                //
                                rule.useCount = 1;
                                nInst.eofAction = rule.aSpan;
                                nInst.MakePath(tree, start, endSt);
                                nInst.MarkAccept(endSt, rule);
                            }
                            else if (tree.op == RegOp.rightAnchor)
                            {
                                tree = ((Unary)tree).kid;
                                nInst.MakePath(tree, start, endSt);
                                AddAnchorContext(nInst, endSt, rule);
                            }
                            else
                            {
                                nInst.MakePath(tree, start, endSt);
                                nInst.MarkAccept(endSt, rule);
                            }
                        }
                    }
                }
            }
            if (task.Verbose)
            {
                Console.Write("GPLEX: NFSA built");
                Console.Write((task.Errors ? ", errors detected" : " without error"));
                Console.Write((task.Warnings ? "; warnings issued. " : ". "));
                Console.WriteLine(TaskState.ElapsedTime(time0));
            }
            if (task.Summary)
                WriteSummary(time0);
        }

        //
        // For version 1.0.1 recognize any line-end character if /unicode
        //
        static void AddAnchorContext(NfsaInstance nInst, NState endS, RuleDesc rule)
        {
            NState nEnd = nInst.MkState();
            Leaf temp = new Leaf(RegOp.charClass);
            temp.rangeLit = RangeLiteral.RightAnchors;
            nInst.MakePath(temp, endS, nEnd);
            nInst.MarkAccept(nEnd, rule);
            nEnd.rhCntx = 1;
        }

        void WriteSummary(DateTime time)
        {
            task.ListStream.WriteLine("/*");
            task.ListDivider();
            task.ListStream.WriteLine("NFSA Summary for input file <" + task.FileName + ">");
            task.ListDivider();
            task.ListStream.WriteLine("Total NFSA states = " + this.next.Count.ToString());
            task.ListStream.WriteLine("Number of Start Conditions = " + (nfas.Length - 1));
            foreach (NfsaInstance inst in nfas)
            {
                if (inst != null)
                {
                    task.ListStream.WriteLine("Start condition " + inst.key + ":");
                    task.ListStream.Write("  number of patterns = " + inst.myStartCondition.rules.Count);
                    task.ListStream.Write(", number of nfsa states = " + inst.nStates.Count);
                    task.ListStream.WriteLine(", accept states = " + inst.acceptStates.Count);
                }
            }

            task.ListDivider();
            task.ListStream.Write("GPLEX: NFSA built. ");
            task.ListStream.WriteLine(TaskState.ElapsedTime(time));
            task.ListStream.Flush();
        }
    }
}
