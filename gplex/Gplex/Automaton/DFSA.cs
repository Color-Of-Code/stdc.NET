// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using QUT.Gplex.Parser;
using QUT.Gplib;

namespace QUT.Gplex.Automaton
{
    /// <summary>
    /// Represents a SET of DFSA. There is a separate DFSA 
    /// instance generated for each separate StartCondition.
    /// That is: there is only one DFSA object with one or
    /// more DfsaInstance objects in the "dfas" field.
    /// </summary>
    internal class DFSA
    {
        public const int unset = -1;
        public const int gotoStart = -1;
        public const int eofNum = 0;

        /// <summary>
        /// The distinguished EOF state.
        /// </summary>
        public DState eofState;

        /// <summary>
        /// The actual count of entries in the transition table.
        /// This takes into account compression (use of default values)
        /// and row-sharing if there are identical rows.
        /// </summary>
        int tranNum;

        /// <summary>
        /// Counter for allocation of final ordinal
        /// numbering to DState objects. 
        /// </summary>
        internal int globNext = eofNum + 1;       // State zero is "eofState"

        int backupCount;                 // Number of states needing backup
        int maxAccept;
        int copyNum;                     // Number of next state entry aliases
        bool useTwoLevelMap;
        internal bool hasLeftAnchors;
        bool foundShortestStrings;

        int maxSym;                      // Backing field for MaxSym property

        /// <summary>
        /// A reference to the controlling task-state
        /// </summary>
        internal TaskState myTask;

        /// <summary>
        /// Array of DFSA instances
        /// </summary>
        DfsaInstance[] dfas;

        /// <summary>
        /// A list of all created DStates
        /// </summary>
        internal List<DState> stateList = new List<DState>();
        internal int origLength;

        /// <summary>
        /// "next" is the global next state table
        /// </summary>
        internal IDictionary<ulong, DState> next = new Dictionary<ulong, DState>();

        /// <summary>
        /// "newNext" is the new dictionary in which the replacement next
        /// state table is built, in the event that the FSA is minimized.
        /// </summary>
        internal IDictionary<ulong, DState> newNext;
        void InitNewNext() { newNext = new Dictionary<ulong, DState>(); }
        void OverwriteOldNext() { next = newNext; }

        public DFSA(TaskState task)
        {
            myTask = task;
            maxSym = (task.ChrClasses ? task.partition.Length : task.TargetSymCardinality);
            eofState = new DState(this);
            eofState.Num = eofNum;
            stateList.Add(eofState);
        }

        /// <summary>
        /// Cardinality of symbol alphabet in next-state tables.
        /// This could be the real alphabet, or the equivalence class cardinality.
        /// </summary>
        internal int MaxSym { get { return maxSym; } }


        /// <summary>
        /// This method computes the shortest string reaching each state of the automaton
        /// </summary>
        void FindShortestStrings()
        {
            Queue<DState> worklist = new Queue<DState>();
            DfsaInstance inst;
            DState elem;
            DState next;
            // Push every start state and anchor state on the worklist
            for (int i = 0; i < dfas.Length; i++)
            {
                inst = dfas[i];
                if (inst != null)
                {
                    inst.start.shortestStr = "";
                    worklist.Enqueue(inst.start);
                    inst.start.listed = true;
                    if (inst.anchor != null)
                    {
                        inst.anchor.shortestStr = "^";
                        worklist.Enqueue(inst.anchor);
                        inst.anchor.listed = true;
                    }
                }
            }
            // Process every state reachable from the popped state
            while (worklist.Any())
            {
                elem = worklist.Dequeue();
                elem.listed = false;
                for (int i = 1; i < MaxSym; i++)
                {
                    int ch = i;
                    next = elem.GetNext(ch);
                    if (next != null &&
                        (next.shortestStr == null || next.shortestStr.Length > elem.shortestStr.Length + 1))
                    {
                        next.shortestStr = elem.shortestStr +
                            (myTask.ChrClasses ? (char)myTask.partition.InvMap(ch) : (char)ch);
                        // need unmapped characters!
                        if (!next.listed)
                        {
                            worklist.Enqueue(next);
                            next.listed = true;
                        }
                    }
                }
            }
            foundShortestStrings = true;
        }

        /// <summary>
        /// Find reaching strings for each backup state
        /// whether shortest strings or not.
        /// </summary>
        void FindReachingStrings()
        {
            Queue<DState> worklist = new Queue<DState>();
            DfsaInstance inst;
            DState elem;
            DState next;
            int count = backupCount;

            // Push every start state and anchor state on the worklist
            for (int i = 0; i < dfas.Length; i++)
            {
                inst = dfas[i];
                if (inst != null)
                {
                    inst.start.shortestStr = "";
                    worklist.Enqueue(inst.start);
                    inst.start.listed = true;
                    if (inst.anchor != null)
                    {
                        inst.anchor.shortestStr = "^";
                        worklist.Enqueue(inst.anchor);
                        inst.anchor.listed = true;
                    }
                }
            }
            // Process every unlisted state reachable from the popped state
            while (worklist.Any())
            {
                elem = worklist.Dequeue();
                for (int i = 1; i < MaxSym; i++)
                {
                    int ch = i;
                    next = elem.GetNext(ch);
                    if (next != null && !next.listed)
                    {
                        next.shortestStr = elem.shortestStr +
                            (myTask.ChrClasses ? (char)myTask.partition.InvMap(ch) : (char)ch);
                        // need unmapped characters!
                        if (!next.listed)
                        {
                            worklist.Enqueue(next);
                            next.listed = true;
                            if (next.needsBackup)
                            {
                                count--;
                                if (count == 0) return;
                            }
                        }
                    }
                }
            }
        }

        internal string MapSymToStr(int code)
        {
            if (this.myTask.ChrClasses)
                code = this.myTask.partition.InvMap(code);
            return CharacterUtilities.QuoteMap(code);
        }

        /// <summary>
        /// Convert the non-deterministic FSA to a deterministic FSA
        /// for each DfsaInstance, i.e. for each StartCondition
        /// </summary>
        /// <param name="nfa">The non-deterministic FSA to convert</param>
        public void Convert(NFSA nfa)
        {
            DateTime start = DateTime.Now;
            dfas = new DfsaInstance[nfa.nfas.Length];
            // Perform subset construction, separately for each NfsaInstance
            for (int i = 0; i < dfas.Length; i++)
            {
                NfsaInstance nInst = nfa.nfas[i];
                if (nInst != null)
                {
                    DfsaInstance dInst = new DfsaInstance(nInst, this);
                    dfas[i] = dInst;
                    dInst.Convert();
                }
            }
            //
            // Now check for any rules that are unreachable.
            //
            foreach (RuleDesc rule in myTask.aast.ruleList)
            {
                if (rule.useCount == 0 && !rule.isPredDummyRule)
                {
                    if (rule.replacedBy != null)
                    {
                        myTask.handler.ListError(rule.replacedBy.pSpan, 117, rule.Pattern, '"');
                        myTask.handler.ListError(rule.pSpan, 116, rule.replacedBy.Pattern, '"');
                    }
                    else
                        myTask.handler.ListError(rule.pSpan, 116);
                }
            }

            if (myTask.Verbose)
            {
                myTask.Msg.Write("GPLEX: DFSA built");
                myTask.Msg.Write((myTask.HasErrors ? ", errors detected" : " without error"));
                myTask.Msg.Write((myTask.HasWarnings ? "; warning(s) issued. " : ". "));
                myTask.Msg.WriteLine(TaskState.ElapsedTime(start));
            }
            if (myTask.Summary)
            {
                myTask.ListDivider();
                myTask.ListStream.Write("GPLEX: DFSA built. ");
                myTask.ListStream.WriteLine(TaskState.ElapsedTime(start));
            }
        }

        /// <summary>
        /// Write summary to the listing file.
        /// </summary>
        void WriteSummary(DateTime time)
        {
            int symCard = myTask.TargetSymCardinality;
            int fullNum = globNext * symCard;
            double totalCompression = 100.0 - (double)(tranNum * 100) / fullNum;

            myTask.ListDivider();
            myTask.ListStream.WriteLine("DFSA Summary");
            myTask.ListDivider();
            myTask.ListStream.WriteLine("Number of dfsa instances = " + (dfas.Length - 1));
            for (int i = 0; i < dfas.Length; i++)
            {
                DfsaInstance inst = dfas[i];
                if (inst != null)
                {
                    myTask.ListStream.WriteLine("Start condition " + inst.myNfaInst.key + ":");
                    myTask.ListStream.Write("  number of dfsa states = " + inst.instNext);
                    myTask.ListStream.WriteLine(", number of accept states = " + inst.acceptCount);
                }
            }
            myTask.ListDivider();
            myTask.ListStream.Write("GPLEX: C# file emitted. ");
            myTask.ListStream.WriteLine(TaskState.ElapsedTime(time));

            myTask.ListDivider();
            myTask.ListStream.WriteLine("GPLEX Summary");
            myTask.ListDivider();
            myTask.ListStream.WriteLine("Total number of states = " + globNext +
                                        ", total accept states = " + maxAccept +
                                        ", backup states = " + backupCount);
            if (hasLeftAnchors)
                myTask.ListStream.WriteLine("Automaton will cater for left-anchored patterns");

            if (myTask.Minimize)
                myTask.ListStream.WriteLine("Original state number was {0}, minimized machine has {1} states",
                    origLength, globNext);
            else
                myTask.ListStream.WriteLine("No state minimization.");

            double nextstateCompression = 100.0 - (double)(tranNum * 100) / (globNext * MaxSym);
            if (myTask.ChrClasses)
            {
                int entries = symCard;
                double classCompression = 100.0 - (double)(MaxSym * 100) / symCard;

                if (myTask.CompressMap)
                {
                    entries = 0;
                    if (this.useTwoLevelMap)
                    {
                        entries = myTask.partition.pages.Count;
                        foreach (MapRun r in myTask.partition.pages)
                            if (r.tag == MapRun.TagType.mixedValues)
                                entries += r.Length;
                    }
                    else
                    {
                        foreach (MapRun r in myTask.partition.mapRuns)
                            if (r.tag == MapRun.TagType.mixedValues)
                                entries += r.Length;
                    }

                }
                myTask.ListStream.WriteLine("Compression summary: used {0:N0} nextstate entries, plus {1:N0} map entries",
                    tranNum, entries);
                myTask.ListStream.WriteLine("- Uncompressed automaton would have {0:N0} nextstate entries", fullNum);
                myTask.ListStream.WriteLine("- Input characters are packed into {0:N0} equivalence classes", MaxSym);
                myTask.ListStream.WriteLine("- CharClass compression {0:F2}%, {1:N0} entries Vs {2:N0}",
                    classCompression, MaxSym, symCard);
                if (myTask.CompressNext)
                {
                    myTask.ListStream.WriteLine("- Nextstate table compression {0:F2}%, {1:N0} entries Vs {2:N0}",
                        nextstateCompression, tranNum, globNext * MaxSym);
                }
                else
                {
                    myTask.ListStream.WriteLine("- Redundant row compression {0:F2}%, {1:N0} entries Vs {2:N0}",
                        nextstateCompression, tranNum, globNext * MaxSym);
                }
                if (myTask.CompressMap)
                {
                    if (this.useTwoLevelMap)
                    {
                        myTask.ListStream.WriteLine(
                            "- Two-level CharacterMap compression is {0:F2}%, {1:N0} entries Vs {2:N0}",
                            100.0 - (double)(entries * 100) / symCard, entries, symCard);
                    }
                    else
                    {
                        int depth = (int)Math.Ceiling(Math.Log(myTask.partition.mapRuns.Count, 2));
                        myTask.ListStream.WriteLine(
                            "- CharacterMap compression is {0:F2}%, {1:N0} entries Vs {2:N0}",
                            100.0 - (double)(entries * 100) / symCard, entries, symCard);
                        myTask.ListStream.WriteLine("- Decision tree depth is {0}", depth);
                    }
                }
                else
                    myTask.ListStream.WriteLine("- ClassMap was not compressed");
            }
            else if (myTask.CompressNext) // compressedNext but no map ...
                myTask.ListStream.WriteLine("Nextstate table compression was {0:F2}%, {1:N0} entries Vs {2:N0}",
                                        totalCompression, tranNum, fullNum);
            else
                myTask.ListStream.WriteLine("- Redundant row compression {0:F2}%, {1:N0} entries Vs {2:N0}",
                        nextstateCompression, tranNum, globNext * MaxSym);

            if (backupCount > 0)
            {
                myTask.ListStream.WriteLine();
                myTask.ListStream.WriteLine("Backup state report --- ");
                for (int i = 0; i <= maxAccept; i++)
                    if (stateList[i].needsBackup)
                    {
                        DState dSt = stateList[i];
                        myTask.ListStream.WriteLine(
                            "In <{0}>, after \"{1}\" automaton could accept \"{3}\" in state {2}",
                            dSt.AbreviatedStartConditionName, CharacterUtilities.Map(dSt.shortestStr), i, dSt.accept.Pattern);
                        myTask.ListStream.WriteLine(
                            "--- after '{0}' automaton is in a non-accept state and might need to backup",
                            dSt.BackupTransition());
                        myTask.ListStream.WriteLine();
                    }
            }
            myTask.ListDivider();
            myTask.ListStream.WriteLine(" */");
            myTask.ListStream.Flush();
        }

        /// <summary>
        /// Minimize the FSA ... using a variant of Hopcroft's algorithm
        /// </summary>
        public void Minimize()
        {
            if (myTask.Minimize)
            {
                DateTime start = DateTime.Now;
                Minimizer mini = new Minimizer(this);
                mini.PopulatePartitions(stateList);
                mini.RefinePartitions();
                this.RewriteStateList();

                if (myTask.Verbose)
                {
                    myTask.Msg.Write("GPLEX: DFSA minimized. ");
                    myTask.Msg.WriteLine(TaskState.ElapsedTime(start));
                }

                if (myTask.Summary)
                {
                    myTask.ListDivider();
                    myTask.ListStream.Write("GPLEX: DFSA minimized. ");
                    myTask.ListStream.WriteLine(TaskState.ElapsedTime(start));
                }
            }
        }

        /// <summary>
        /// Rewrite the automaton to use the minimized states.
        /// Each partition in the final map is a DFSA state
        /// in the minimal machine.
        /// </summary>
        /// <param name="list">The minimizer object</param>
        public void RewriteStateList()
        {
            var newList = new List<DState>();
            InitNewNext();
            foreach (DfsaInstance inst in dfas)
            {
                if (inst != null)
                {
                    inst.start = Minimizer.PMap(inst.start);
                    if (inst.anchor != null)
                        inst.anchor = Minimizer.PMap(inst.anchor);
                }
            }
            newList.Add(eofState);
            globNext = eofNum + 1;               // All accept state get renumbered.
            for (int idx = 1; idx < stateList.Count; idx++)
            {
                DState dSt = stateList[idx];
                DState pSt = Minimizer.PMap(dSt);
                if (dSt == pSt)
                {
                    newList.Add(pSt);
                    if (pSt.accept != null)
                        pSt.Num = globNext++;
                    for (int sym = 0; sym < this.MaxSym; sym++)
                    {
                        DState nxt = pSt.GetNext(sym);
                        if (nxt != null)
                            // Set value in *new* next-state table
                            pSt.SetNewNext(sym, Minimizer.PMap(nxt));
                    }
                }
            }
            stateList = newList; // swap old for new stateList
            OverwriteOldNext();  // replace old for new next-state table
        }

        /// <summary>
        /// Emit the scanner to the output file
        /// </summary>
        /// <param name="sRdr">the reader for the frame file</param>
        /// <param name="sWrtr">the writer for the output C# file</param>
        public void EmitScanner(TextReader sRdr, TextWriter sWrtr)
        {
            DateTime start = DateTime.Now;
            int frameVersion = 0;

            if (sRdr != null && sWrtr != null)
            {
                int[] startMap = new int[dfas.Length];
                int[] anchorMap = new int[dfas.Length];

                string line;
                // Write the expanatory header
                sWrtr.WriteLine("//");
                sWrtr.WriteLine("//  This CSharp output file generated by Gardens Point LEX");
                sWrtr.WriteLine("//  Gardens Point LEX (GPLEX) is Copyright (c) John Gough, QUT 2006-2014.");
                sWrtr.WriteLine("//  Output produced by GPLEX is the property of the user.");
                sWrtr.WriteLine("//  See accompanying file GPLEXcopyright.rtf.");
                sWrtr.WriteLine("//");
                sWrtr.WriteLine("//  GPLEX Version:  " + myTask.VerString);
                if (myTask.EmitInfoHeader)
                {
                    sWrtr.WriteLine("//  Machine:  " + Environment.MachineName);
                    sWrtr.WriteLine("//  DateTime: " + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    sWrtr.WriteLine("//  UserName: " + Environment.UserName);
                }
                sWrtr.WriteLine("//  GPLEX input file <" + myTask.InputInfo + ">");
                sWrtr.WriteLine("//  GPLEX frame file <" + myTask.FrameName + ">");
                sWrtr.WriteLine("//");
                sWrtr.WriteLine("//  Option settings:{0}{1}{2}{3}{4}{5}",
                    (myTask.UseUnicode ? " unicode," : ""),
                    (myTask.Verbose ? " verbose," : ""),
                    (myTask.HasParser ? " parser," : " noParser,"),
                    (myTask.Files ? "" : " noFiles,"),
                    (myTask.Stack ? " stack," : ""),
                    (myTask.Minimize ? " minimize" : " noMinimize"));
                sWrtr.WriteLine("//  Option settings:{0}{1}{2}{3}{4}",
                    (myTask.ChrClasses ? " classes," : ""),
                    (myTask.ChrClasses ? (myTask.CompressMap ? " compressMap," : " noCompressMap,") : ""),
                    (myTask.CompressNext ? " compressNext," : " noCompressNext,"),
                    (myTask.Files ? (myTask.Persist ? " persistBuffer," : " noPersistBuffer,") : ""),
                    (myTask.EmbedBuffers ? " embedbuffers" : " noEmbedBuffers"));

                if (myTask.UseUnicode && myTask.Files)
                {
                    string page = null;
                    switch (myTask.CodePage)
                    {
                        case TaskState.rawCP:
                            page = "Raw 8-bit binary"; break;
                        case TaskState.defaultCP:
                            page = "Target machine default"; break;
                        case TaskState.utf8CP:
                            page = "utf-8"; break;
                        case 1200:
                            page = "utf-16 (Little Endian)"; break;
                        case 1201:
                            page = "utf-16 (Big Endian)"; break;
                        case TaskState.guessCP:
                            page = "Scan the file, then guess"; break;
                        default:
                            page = myTask.CodePage.ToString(CultureInfo.InvariantCulture); break;
                    }
                    sWrtr.WriteLine("//  Fallback code page: {0}", page);
                }
                sWrtr.WriteLine("//");
                sWrtr.WriteLine();
                // Number, and then sort the dfsa states according to global numbering
                maxAccept = globNext - 1;
                for (int i = 0; i < stateList.Count; i++)
                {
                    DState dSt = stateList[i];
                    if (dSt.Num == unset)
                        dSt.Num = globNext++;
                }
                stateList.Sort();           // sorted on globOrd number
                // Only check the accept states. Backup transitions can only start here.
                for (int i = 0; i <= maxAccept; i++)
                {
                    if (stateList[i].NeedsBackup())
                    {
                        stateList[i].needsBackup = true;
                        backupCount++;
                    }
                }

                if (myTask.Verbose)
                {
                    DateTime begin = DateTime.Now;
                    FindShortestStrings();
                    // Substract the elapsed time for computing the
                    // shortest strings from the C# file emission time.
                    start.Add(DateTime.Now - begin);
                    myTask.Msg.Write("GPLEX: Shortest strings found. ");
                    myTask.Msg.WriteLine(TaskState.ElapsedTime(begin));
                    if (myTask.Summary)
                    {
                        myTask.ListDivider();
                        myTask.ListStream.Write("GPLEX: Shortest strings found. ");
                        myTask.ListStream.WriteLine(TaskState.ElapsedTime(begin));
                    }
                }

                //
                // Now the loop that copies the frame file line by line,
                // interleaving the generated material when required.
                //
                while ((line = sRdr.ReadLine()) != null)
                {
                    if (line.StartsWith("##", StringComparison.Ordinal))
                    {
                        if (line.StartsWith("##-->", StringComparison.Ordinal))
                        {
                            string selector = line.Substring(5, line.Length - 5).Trim();

                            if (selector == "defines")
                            {
                                if (backupCount > 0)
                                    sWrtr.WriteLine("#define BACKUP");
                                if (hasLeftAnchors)
                                    sWrtr.WriteLine("#define LEFTANCHORS");
                                if (myTask.Stack)
                                    sWrtr.WriteLine("#define STACK");
                                if (!myTask.HasParser)
                                    sWrtr.WriteLine("#define STANDALONE");
                                if (myTask.Babel)
                                    sWrtr.WriteLine("#define BABEL");
                                if (!myTask.Files)
                                    sWrtr.WriteLine("#define NOFILES");
                                if (myTask.Persist)
                                    sWrtr.WriteLine("#define PERSIST");
                                if (!myTask.UseUnicode)
                                    sWrtr.WriteLine("#define BYTEMODE");
                                for (int i = 0; i < dfas.Length; i++)
                                    if (dfas[i] != null)
                                    {
                                        startMap[dfas[i].StartConditionOrd] = dfas[i].start.Num;
                                        anchorMap[dfas[i].StartConditionOrd] =
                                            (dfas[i].anchor == null ? dfas[i].start.Num : dfas[i].anchor.Num);
                                    }
                            }
                            if (selector.StartsWith("version", StringComparison.Ordinal))
                            {
                                try
                                {
                                    frameVersion = int.Parse(selector.Substring(7, selector.Length - 7), CultureInfo.InvariantCulture);
                                }
                                catch (ArgumentException) { }
                                catch (OverflowException) { }
                                catch (FormatException) { }
                            }
                            else if (selector == "usingDcl")
                            {
                                if (!myTask.EmbedBuffers)
                                    sWrtr.WriteLine("using QUT.GplexBuffers;");

                                foreach (LexSpan s in myTask.aast.usingStrs)
                                {
                                    sWrtr.Write("using ");
                                    s.StreamDump(sWrtr);
                                }
                                sWrtr.WriteLine();
                                sWrtr.Write("namespace ");
                                myTask.aast.nameString.StreamDump(sWrtr);
                                // 
                                //  version marker must occur before usingDcl
                                //  so check now if gplexx version is ok
                                //
                                if (frameVersion < TaskState.minGplexxVersion)
                                {
                                    myTask.handler.ListError(myTask.aast.AtStart, 91);
                                    throw new IOException("Wrong version of frame file");
                                }
                            }
                            else if (selector == "codeIncl")
                            {
                                foreach (LexSpan s in myTask.aast.CodeIncl)
                                    s.StreamDump(sWrtr);
                                if (myTask.aast.HasPredicates)
                                    foreach (LexCategory cat in myTask.aast.lexCatsWithPredicates)
                                        EmitPredicate(sWrtr, cat);
                            }
                            else if (selector == "tableDef")
                            {
                                if (myTask.CompressNext)
                                    EmitSlicedTables(sWrtr);
                                else
                                    EmitRawTables(sWrtr);
                            }
                            else if (selector == "prolog")
                            {
                                if (myTask.aast.Prolog.Any())
                                {
                                    sWrtr.WriteLine("// User-specified prolog to scan()");
                                    foreach (LexSpan s in myTask.aast.Prolog)
                                        s.StreamDump(sWrtr);
                                    sWrtr.WriteLine("// End, user-specified prolog");
                                }
                                if (myTask.aast.Epilog.Any())
                                    sWrtr.WriteLine("            try {");
                            }
                            else if (selector == "consts")
                            {
                                sWrtr.WriteLine("        const int maxAccept = " + maxAccept + ";");
                                sWrtr.WriteLine("        const int initial = " + startMap[0] + ";");
                                sWrtr.WriteLine("        const int eofNum = " + eofNum + ";");
                                sWrtr.WriteLine("        const int goStart = " + gotoStart + ";");
                                for (int i = 0; i < dfas.Length; i++)
                                    if (dfas[i] != null)
                                        sWrtr.WriteLine(String.Format(CultureInfo.InvariantCulture, "        const int {0} = {1};",
                                                                      dfas[i].StartConditionName,
                                                                      dfas[i].StartConditionOrd));
                            }
                            else if (selector == "actionCases")
                                EmitActionCases(sWrtr, maxAccept);
                            else if (selector == "epilog" && myTask.aast.Epilog.Any())
                            {
                                sWrtr.WriteLine("            } // end try");
                                sWrtr.WriteLine("            finally {");
                                sWrtr.WriteLine("// User-specified epilog to scan()");
                                foreach (LexSpan s in myTask.aast.Epilog)
                                    s.StreamDump(sWrtr);
                                sWrtr.WriteLine("// End, user-specified epilog");
                                sWrtr.WriteLine("            } // end finally");
                            }
                            else if (selector == "userCode" &&
                                myTask.aast.UserCode != null &&
                                myTask.aast.UserCode.IsInitialized)
                            {
                                sWrtr.WriteLine("#region UserCodeSection");
                                sWrtr.WriteLine();
                                myTask.aast.UserCode.StreamDump(sWrtr);
                                sWrtr.WriteLine();
                                sWrtr.WriteLine("#endregion");
                            }
                            else if (selector == "embeddedBuffers")
                            {
                                if (myTask.EmbedBuffers)
                                    TaskState.EmbedBufferCode(sWrtr);
                            }
                            else if (selector == "bufferCtor")
                            {
                                if (myTask.UseUnicode)
                                {
                                    sWrtr.WriteLine("            SetSource(file, {0}); // unicode option", myTask.CodePage);
                                    sWrtr.WriteLine("        }");
                                    sWrtr.WriteLine();
                                    sWrtr.WriteLine("        public {0}(Stream file, string codepage) {{", myTask.aast.scannerTypeName);
                                    sWrtr.WriteLine("            SetSource(file, CodePageHandling.GetCodePage(codepage));");
                                }
                                else
                                    sWrtr.WriteLine("            SetSource(file); // no unicode option");

                            }
                            else if (selector.StartsWith("visibility", StringComparison.Ordinal))
                            {
                                sWrtr.Write("    ");
                                sWrtr.WriteLine(Translate(selector));
                            }
                            else if (selector.StartsWith("translate", StringComparison.Ordinal))
                            {
                                sWrtr.Write("    ");
                                sWrtr.WriteLine(Translate(selector.Substring(9)));
                            }
                        }
                        // Else this line is script comment, do no copy
                    }
                    else
                        sWrtr.WriteLine(line);
                }
                if (myTask.Summary)
                {
                    if (backupCount > 0 && !foundShortestStrings)
                    {
                        DateTime begin = DateTime.Now;
                        FindReachingStrings();
                        if (myTask.Verbose)
                        {
                            myTask.Msg.Write("GPLEX: Reaching strings found. ");
                            myTask.Msg.WriteLine(TaskState.ElapsedTime(begin));
                        }
                        myTask.ListDivider();
                        myTask.ListStream.Write("GPLEX: Reaching strings found. ");
                        myTask.ListStream.WriteLine(TaskState.ElapsedTime(begin));
                        start = start.Add(DateTime.Now - begin);
                    }
                    WriteSummary(start);
                }
                if (myTask.Verbose)
                {
                    myTask.Msg.Write("GPLEX: C# file emitted. ");
                    myTask.Msg.WriteLine(TaskState.ElapsedTime(start));
                }
                sWrtr.Flush();
            }
        }

        private string Translate(string originalText)
        {
            string result = originalText.Replace("$public", myTask.aast.visibility);
            result = result.Replace("$ScanBase", myTask.aast.scanBaseName);
            result = result.Replace("$Scanner", myTask.aast.scannerTypeName);
            result = result.Replace("$Tokens", myTask.aast.tokenTypeName);
            return result;
        }

        internal static void EmitPredicate(TextWriter sWrtr, LexCategory cat)
        {
            sWrtr.WriteLine("        // Character Set Predicate (auto-generated)");
            sWrtr.WriteLine("        public bool Is_{0}(int val) {{", cat.Name);
            sWrtr.WriteLine("            int here = startState[{0}];", cat.PredDummyName);
            sWrtr.WriteLine("            return TestNextState(here, val) == here;");
            sWrtr.WriteLine("        }");
            sWrtr.WriteLine();
        }


        /// <summary>
        /// Emit the semantic actions for the recognized patterns.
        /// </summary>
        /// <param name="sWrtr">the stream writer</param>
        /// <param name="max">the max accept ordinal</param>
        internal void EmitActionCases(TextWriter sWrtr, int max)
        {
            int eofCount = 0;
            bool[] emitted = new bool[max + 1];
            sWrtr.WriteLine("#region ActionSwitch");
            sWrtr.WriteLine("#pragma warning disable 162, 1522");
            sWrtr.WriteLine("    switch (state)");
            sWrtr.WriteLine("    {");
            sWrtr.WriteLine("        case eofNum:");
            //
            //  Must check if there are any explicit EOF actions
            //
            for (int i = 0; i < dfas.Length; i++)
                if (dfas[i] != null && dfas[i].eofCode != null && dfas[i].eofCode.IsInitialized) eofCount++;
            if (eofCount >= 1)
            {
                bool[] eofDone = new bool[dfas.Length];
                bool[] stateDone = new bool[globNext];

                sWrtr.WriteLine("            switch (currentStart) {");
                for (int i = 0; i < dfas.Length; i++)
                {
                    DfsaInstance dInst = dfas[i];
                    if (dInst != null && dInst.eofCode != null && dInst.eofCode.IsInitialized && !eofDone[i])
                    {
                        int d = dInst.start.Num;
                        eofDone[i] = true;
                        stateDone[d] = true;
                        sWrtr.WriteLine("                case " + d + ":");
                        //
                        //  We wish to share the same action text spans
                        //  for all equivalent eof-actions.
                        //  Note that the test for equivalent actions is
                        //  simpler than the general case, since eof-actions
                        //  cannot have right (or for that matter left) context.
                        //
                        //  There is a special case here, where two start conditions
                        //  might share the same start state as a result of minimization.
                        //  So, we must guard against duplicates in the switch statement.
                        //
                        for (int j = i + 1; j < dfas.Length; j++)
                        {
                            DfsaInstance nInst = dfas[j];
                            if (nInst != null &&
                                nInst.eofCode != null &&
                                nInst.eofCode.IsInitialized &&
                                !eofDone[j] &&
                                SpansEqual(dInst.eofCode, nInst.eofCode))
                            {
                                int n = nInst.start.Num;
                                eofDone[j] = true;
                                if (!stateDone[n])
                                {
                                    stateDone[n] = true;
                                    sWrtr.WriteLine("                case " + n + ":");
                                }
                            }
                        }
                        dInst.eofCode.StreamDump(sWrtr);
                        sWrtr.WriteLine("                    break;");
                    }
                }
                sWrtr.WriteLine("            }");
            }
            sWrtr.WriteLine("            if (yywrap())");
            sWrtr.WriteLine("                return (int){0}.EOF;", myTask.aast.tokenTypeName);
            sWrtr.WriteLine("            break;");
            //
            //  Many states may share the same actions so reuse
            //  the code whenever possible. 
            //  This sharing arises in two ways:
            //  explicit use of the '|' action in the LEX
            //  file, and splitting of an accept state during
            //  construction of the automaton.
            //
            for (int sOrd = eofNum + 1; sOrd <= max; sOrd++)
            {
                DState dSt = stateList[sOrd];
                if (!emitted[sOrd])
                {
                    int rLen = dSt.rhCntx;
                    int lLen = dSt.lhCntx;
                    string caselabel0 = "        case " + dSt.Num + ":";
                    sWrtr.Write(caselabel0);
                    if (myTask.Verbose)
                    {
                        sWrtr.Write(" // ");
                        if (dSt.myDfaInst.StartConditionOrd != 0)
                            sWrtr.Write("In <{0}> ", dSt.AbreviatedStartConditionName);
                        sWrtr.Write("Recognized '{0}'", dSt.accept.Pattern);
                        if (foundShortestStrings)
                            sWrtr.Write(",\tShortest string \"{0}\"", CharacterUtilities.Map(dSt.shortestStr));
                    }
                    sWrtr.WriteLine();
                    emitted[sOrd] = true;
                    for (int j = sOrd; j <= max; j++)
                    {
                        DState nSt = stateList[j];
                        if (!emitted[j] &&
                            lLen == nSt.lhCntx && rLen == nSt.rhCntx &&
                             SpansEqual(dSt.accept.aSpan, nSt.accept.aSpan))
                        {
                            string caselabelN = "        case " + nSt.Num + ":";
                            sWrtr.Write(caselabelN);
                            if (myTask.Verbose)
                            {
                                sWrtr.Write(" // ");
                                if (nSt.myDfaInst.StartConditionOrd != 0)
                                    sWrtr.Write("In <{0}> ", nSt.AbreviatedStartConditionName);
                                sWrtr.Write("Recognized '{0}'", nSt.accept.Pattern);
                                if (foundShortestStrings)
                                    sWrtr.Write(",\tShortest string \"{0}\"", CharacterUtilities.Map(nSt.shortestStr));
                            }
                            sWrtr.WriteLine();
                            emitted[j] = true;
                        }
                    }
                    if (lLen > 0) sWrtr.WriteLine("yyless({0}); ", lLen);
                    else if (rLen > 0) sWrtr.WriteLine("_yytrunc({0}); ", rLen);
                    if (dSt.accept.HasAction)
                        dSt.accept.aSpan.StreamDump(sWrtr);
                    sWrtr.WriteLine("            break;");
                }
            }
            sWrtr.WriteLine("        default:");
            sWrtr.WriteLine("            break;");
            sWrtr.WriteLine("    }");
            sWrtr.WriteLine("#pragma warning restore 162, 1522");
            sWrtr.WriteLine("#endregion");
        }

        public static bool SpansEqual(LexSpan l, LexSpan r)
        {
            return l.startIndex == r.startIndex && l.endIndex == r.endIndex;
        }

        #region ClassMapHandling
        /// <summary>
        /// Write an uncompressed map from character to 
        /// equivalence class ordinal.
        /// </summary>
        /// <param name="sWrtr">The output stream writer</param>
        private void WriteClassMap(TextWriter sWrtr)
        {
            string mapTyNm;
            int domain = myTask.TargetSymCardinality;
            int range = myTask.partition.Length;
            if (range < sbyte.MaxValue)
                mapTyNm = "sbyte";
            else if (range < short.MaxValue)
                mapTyNm = "short";
            else
                mapTyNm = "int";
            sWrtr.WriteLine("#region CharacterMap");
            sWrtr.WriteLine("    static {0}[] map = new {0}[{1}] {{",
                mapTyNm, domain);
            for (int i = 0; i < domain - 1; i++)
            {
                if ((i % 16) == 0)
                    sWrtr.Write("/* {0,8} */ ", CharacterUtilities.QuoteMap(i));
                sWrtr.Write("{0}, ", myTask.partition[i]);
                if ((i % 16) == 15)
                    sWrtr.WriteLine();
            }
            sWrtr.Write("{0} ", myTask.partition[(domain - 1)]);
            sWrtr.WriteLine("};");
            sWrtr.WriteLine("#endregion");
            sWrtr.WriteLine();
        }

        /// <summary>
        /// Write a compressed map from character
        /// to equivalence class ordinal.
        /// </summary>
        /// <param name="sWrtr">The output textwriter</param>
        private void WriteCompressedMap2(TextWriter sWrtr)
        {
            if (useTwoLevelMap)
            {
                myTask.partition.ComputePages();
                WriteTwoLevelMap(sWrtr);
            }
            else
            {
                WriteCompressedMap(sWrtr, myTask.partition.mapRuns);
            }
        }

        /// <summary>
        /// Write out a two-level map sharing pages if possible
        /// </summary>
        /// <param name="sWrtr">The TextWriter to write to</param>
        private void WriteTwoLevelMap(TextWriter sWrtr)
        {
            string mapTyNm;
            int runs = 0;                   // Number of constant runs
            int total = myTask.partition.pages.Count;

            // Determine if we need to emit a compressed table for 
            // the non-BMP planes.  If there are several MapRun OR
            // if there is a single MapRun, but it has a mixed tag.
            // The second disjunct is almost impossible in practice,
            // but, hey, we want to be correct.
            bool needCmpMap =
                myTask.partition.runsInNonBMPs.Count > 1 ||
                myTask.partition.runsInNonBMPs[0].tag == MapRun.TagType.mixedValues;

            // int symCrd = myTask.TargetSymCardinality;
            int range = myTask.partition.Length;
            if (range < sbyte.MaxValue)
                mapTyNm = "sbyte";
            else if (range < short.MaxValue)
                mapTyNm = "short";
            else
                mapTyNm = "int";

            FindAliasMapRuns(myTask.partition);
            for (int i = 0; i < total; i++)
                if (myTask.partition.pages[i].TableOrd == i)
                    runs++;

            sWrtr.WriteLine("#region TwoLevelCharacterMap");
            sWrtr.WriteLine("    //");
            sWrtr.WriteLine("    // There are {0} equivalence classes", range);
            sWrtr.WriteLine("    // There are {0} character sequence regions", total);
            sWrtr.WriteLine("    // There are {0} tables, {1} entries", runs, runs * Partition.PageSize);
            sWrtr.WriteLine("    //");
            for (int n = 0; n < total; n++)
            {
                MapRun r = myTask.partition.pages[n];

                if (r.TableOrd == n) // Only emit defining occurrences
                {
                    sWrtr.WriteLine("    static {0}[] mLo{2} = new {0}[{1}] {{",
                        mapTyNm, r.Length, n);
                    for (int i = 0; i < r.Length - 1; i++)
                    {
                        int j = i + (int)r.range.minChr;
                        if ((i % 16) == 0)
                            sWrtr.Write("/* {0,8} */ ", CharacterUtilities.QuoteMap(j));
                        sWrtr.Write("{0}, ", myTask.partition[j]);
                        if ((i % 16) == 15)
                            sWrtr.WriteLine();
                    }
                    sWrtr.Write("{0} ", myTask.partition[r.range.maxChr]);
                    sWrtr.WriteLine("};");
                }
            }
            sWrtr.WriteLine();

            EmitUpperLevelMap(sWrtr, mapTyNm, total);
            sWrtr.WriteLine();

            sWrtr.WriteLine("#endregion");
            sWrtr.WriteLine();

            if (needCmpMap)
                WriteCompressedMap(sWrtr, myTask.partition.runsInNonBMPs);

            sWrtr.WriteLine();
            sWrtr.WriteLine("    static {0} Map(int code)", mapTyNm);
            sWrtr.WriteLine("    { ");
            sWrtr.WriteLine("        if (code <= {0})", (int)Char.MaxValue);
            sWrtr.WriteLine(String.Format(CultureInfo.InvariantCulture,
                "            return map[code / {0}][code % {0}];", Partition.PageSize));
            sWrtr.WriteLine("        else");
            if (needCmpMap)
                sWrtr.WriteLine("            return MapC(code);");
            else
            {
                int mapOfMin = myTask.partition[Char.MaxValue + 1];
                sWrtr.WriteLine("            return ({0}){1};", mapTyNm, mapOfMin);
            }
            sWrtr.WriteLine("    }");
            sWrtr.WriteLine();
        }

        private void EmitUpperLevelMap(TextWriter sWrtr, string mapTyNm, int total)
        {
            sWrtr.Write("    static {0}[][] map = new {0}[{1}][] {{", mapTyNm, total);
            for (int page = 0; page < total; page++)
            {
                int index = myTask.partition.pages[page].TableOrd;

                if (page % 16 == 0)
                    sWrtr.Write("{0}/* '\\u{1:X2}xx' */ ", Environment.NewLine, page);
                sWrtr.Write("mLo{0}", index);
                if (page + 1 < total)
                    sWrtr.Write(", ");
                else
                    sWrtr.WriteLine("};");
            }
        }

        /// <summary>
        /// Find a lower numbered page which is equal so that
        /// the two table entries can alias the same page reference.
        /// </summary>
        /// <param name="partition">The partition with the page table</param>
        private static void FindAliasMapRuns(Partition partition)
        {
            for (int index = 0; index < partition.pages.Count; index++)
            {
                MapRun indexRun = partition.pages[index];
                indexRun.TableOrd = index; // Set default ordinal tag
                for (int preIx = 0; preIx < index; preIx++)
                {
                    MapRun preIxRun = partition.pages[preIx];
                    if (partition.EqualMaps(indexRun, preIxRun))
                    {
                        indexRun.TableOrd = preIx;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Write out a class map compressed by use of a
        /// decision tree with dense arrays at the leaves.
        /// </summary>
        /// <param name="sWrtr">output stream</param>
        /// <param name="mapRuns">list of dense regions</param>
        private void WriteCompressedMap(TextWriter sWrtr, IList<MapRun> mapRuns)
        {
            string mapTyNm;
            int mixs = 0;                   // Number of mixed runs
            int runs = 0;                   // Number of constant runs
            int sglt = 0;                   // Number of singletons
            int total = mapRuns.Count;
            int entries = 0;
            int symCrd = myTask.TargetSymCardinality;
            int range = myTask.partition.Length;
            int begin = mapRuns[0].range.minChr;
            int depth = (int)Math.Ceiling(Math.Log(mapRuns.Count, 2));

            if (range < sbyte.MaxValue)
                mapTyNm = "sbyte";
            else if (range < short.MaxValue)
                mapTyNm = "short";
            else
                mapTyNm = "int";

            foreach (MapRun r in mapRuns)
            {
                switch (r.tag)
                {
                    case MapRun.TagType.mixedValues:
                        mixs++;
                        entries += r.Length;
                        break;
                    case MapRun.TagType.shortRun:
                    case MapRun.TagType.longRun:
                        runs++;
                        break;
                    case MapRun.TagType.singleton:
                        sglt++;
                        break;
                    default:
                        break;
                }
            }

            sWrtr.WriteLine("#region CompressedCharacterMap");
            sWrtr.WriteLine("    //");
            sWrtr.WriteLine("    // There are {0} equivalence classes", range);
            sWrtr.WriteLine("    // There are {0} character sequence regions", total);
            sWrtr.WriteLine("    // There are {0} tables, {1} entries", mixs, entries);
            sWrtr.WriteLine("    // There are {0} runs, {1} singletons", runs, sglt);
            sWrtr.WriteLine("    // Decision tree depth is {0}", depth);
            sWrtr.WriteLine("    //");
            for (int n = 0; n < total; n++)
            {
                MapRun r = mapRuns[n];
                if (r.tag == MapRun.TagType.mixedValues)
                {
                    sWrtr.WriteLine("    static {0}[] mapC{2} = new {0}[{1}] {{",
                        mapTyNm, r.Length, n);
                    for (int i = 0; i < r.Length - 1; i++)
                    {
                        int j = i + (int)r.range.minChr;
                        if ((i % 16) == 0)
                            sWrtr.Write("/* {0,8} */ ", CharacterUtilities.QuoteMap(j));
                        sWrtr.Write("{0}, ", myTask.partition[j]);
                        if ((i % 16) == 15)
                            sWrtr.WriteLine();
                    }
                    sWrtr.Write("{0} ", myTask.partition[r.range.maxChr]);
                    sWrtr.WriteLine("};");
                }
            }
            sWrtr.WriteLine();
            sWrtr.WriteLine("    static {0} MapC(int code)", mapTyNm);
            sWrtr.Write("    { ");
            EmitDecisionCode(sWrtr, mapTyNm, mapRuns, 4, begin, symCrd - 1, 0, total - 1);
            sWrtr.WriteLine();
            sWrtr.WriteLine("    }");
            sWrtr.WriteLine("#endregion");
            sWrtr.WriteLine();
        }

        /// <summary>
        /// Emit the decision code tree that selects the
        /// applicable run in the equivalence class map.
        /// The algorithm takes a sub-range of the partition run array,
        /// that applies between known min and max character bounds.
        /// The algorithm bisects the subrange and recurses, until a
        /// subrange of just a single run is encountered.
        /// </summary>
        /// <param name="sWrtr">The output text writer</param>
        /// <param name="mapTyNm">Text name of the table element type</param>
        /// <param name="indent">Indent depth for formatting lines of code</param>
        /// <param name="min">Minimum char value treated by this iteration</param>
        /// <param name="max">Maximum char value treated by this iteration</param>
        /// <param name="first">Lowest numbered run to consider</param>
        /// <param name="last">Highest numbered run to consider</param>
        private void EmitDecisionCode(
            TextWriter sWrtr, string mapTyNm, IList<MapRun> mapRuns, int indent, int min, int max, int first, int last)
        {
            sWrtr.Write("// '{0}' <= code <= '{1}'", CharacterUtilities.Map(min), CharacterUtilities.Map(max));
            Indent(sWrtr, indent + 2);
            if (last == first)
            {
                int mapOfMin = myTask.partition[min];
                if (min == max) // this is a singleton;
                    sWrtr.Write("return ({0}){1};", mapTyNm, mapOfMin);
                else if (mapRuns[first].tag == MapRun.TagType.mixedValues)
                    sWrtr.Write("return mapC{0}[code - {1}];", first, min);
                else
                    sWrtr.Write("return ({0}){1};", mapTyNm, mapOfMin);
            }
            else
            {
                int midRun = (first + last + 1) / 2;
                int midPoint = mapRuns[midRun].range.minChr;
                sWrtr.Write("if (code < {0}) ", midPoint);
                EmitDecisionCode(sWrtr, mapTyNm, mapRuns, indent + 2, min, midPoint - 1, first, midRun - 1);
                Indent(sWrtr, indent + 2); sWrtr.Write("else ");
                EmitDecisionCode(sWrtr, mapTyNm, mapRuns, indent + 2, midPoint, max, midRun, last);
            }
        }

        private static void Indent(TextWriter sWrtr, int count)
        {
            sWrtr.WriteLine();
            for (int i = 0; i < count; i++) sWrtr.Write(" ");
        }
        #endregion

        /// <summary>
        /// Write out the scanner tables: this version emits sliced tables
        /// with the longest same-next-state run excluded from the table.
        /// "Longest Run" takes into account wrap-around from MaxChar to
        /// chr(0).
        /// </summary>
        /// <param name="sWrtr">The output text writer</param>
        internal void EmitSlicedTables(TextWriter sWrtr)
        {
            // bool isByte = stateList.Count < 128;
            bool doMap = myTask.ChrClasses;
            bool bigMap = doMap && myTask.CompressMap;
            string eType; // = ("sbyte" or "short" or "int");
            string symStr; // = ("code" or "Map(code)" or "MapC(code)" or "map[code]")

            if (stateList.Count < 128) eType = "sbyte";
            else if (stateList.Count < 0x8000) eType = "short";
            else eType = "int";

            if (bigMap && !myTask.Squeeze)
                useTwoLevelMap = (int)Math.Ceiling(Math.Log(myTask.partition.mapRuns.Count, 2)) > 3;

            if (!doMap)
                symStr = "code";
            else if (useTwoLevelMap) // Two-level map with decision tree above the BMP 
                symStr = "Map(code)";
            else if (bigMap) // The decision-tree case
                symStr = "MapC(code)";
            else             // The uncompressed map
                symStr = "map[code]";

            sWrtr.WriteLine("#region ScannerTables");
            sWrtr.WriteLine("    struct Table {");
            sWrtr.WriteLine("        public int min; public int rng; public int dflt;");
            sWrtr.WriteLine("        public {0}[] nxt;", eType);
            sWrtr.WriteLine("        public Table(int m, int x, int d, {0}[] n) {{", eType);
            sWrtr.WriteLine("            min = m; rng = x; dflt = d; nxt = n;");
            sWrtr.WriteLine("        }");
            sWrtr.WriteLine("    };"); sWrtr.WriteLine();
            //
            // Emit the start state index for each StartCondition.
            //
            sWrtr.Write("    static int[] startState = new int[] {");
            for (int i = 0; i < dfas.Length; i++)
            {
                DfsaInstance dInst = dfas[i];
                sWrtr.Write((dInst == null ? eofNum : dInst.start.Num));
                if (i < dfas.Length - 1)
                {
                    sWrtr.Write(", ");
                    if (i % 16 == 5) { sWrtr.WriteLine(); sWrtr.Write("        "); }
                }
            }
            sWrtr.WriteLine("};"); sWrtr.WriteLine();
            //
            // Emit the left anchored state index for each StartCondition.
            //
            if (hasLeftAnchors)
            {
                sWrtr.Write("   static int[] anchorState = new int[] {");
                for (int i = 0; i < dfas.Length; i++)
                {
                    DfsaInstance dInst = dfas[i];
                    int anchorOrd = eofNum;
                    if (dInst != null)
                        anchorOrd = (dInst.anchor == null ? dInst.start.Num : dInst.anchor.Num);
                    sWrtr.Write((dInst == null ? eofNum : anchorOrd));
                    if (i < dfas.Length - 1)
                    {
                        sWrtr.Write(", ");
                        if (i % 16 == 5) { sWrtr.WriteLine(); sWrtr.Write("        "); }
                    }
                }
                sWrtr.WriteLine("};"); sWrtr.WriteLine();
            }

            if (bigMap)
                WriteCompressedMap2(sWrtr);
            else if (myTask.ChrClasses)
                WriteClassMap(sWrtr);
            // ==========================================================================
            sWrtr.WriteLine("    static Table[] NxS = new Table[{0}] {{", stateList.Count);
            for (int i = 0; i < stateList.Count; i++)
            {
                DState dSt = stateList[i];
                int stDef = dSt.DefaultNext;
                sWrtr.Write("/* NxS[{0,4:D}] */ ", i);
                if (dSt.trList.Count == 0)
                {
                    sWrtr.Write("new Table(0, 0, {0}, null),", stDef);
                    if (myTask.Verbose && foundShortestStrings)
                        sWrtr.Write(" // Shortest string \"{0}\"{1}", CharacterUtilities.Map(dSt.shortestStr), Environment.NewLine);
                    else
                        sWrtr.WriteLine();
                }
                else
                {
                    int dflt = 0;   // The excluded nextstate value.
                    uint min = 0;   // Start index of remainder, in [0-255]
                    uint rng = 0;   // Number of elements in the remainder
                    if (myTask.Verbose && foundShortestStrings)
                        sWrtr.Write("// Shortest string \"{0}\"{1}      ", CharacterUtilities.Map(dSt.shortestStr), Environment.NewLine);
                    dSt.ExcludeLongestRun(out min, out rng, out dflt);
                    tranNum += (int)rng;
                    sWrtr.Write("new Table({0}, {1}, {2}, new {3}[] {{", min, rng, dflt, eType);
                    for (uint j = 0; j < rng; j++)
                    {
                        sWrtr.Write(dSt.NextOn((int)((j + min) % this.MaxSym)));
                        if (j < rng - 1)
                        {
                            sWrtr.Write(", ");
                            if (j % 16 == 5) { sWrtr.WriteLine(); sWrtr.Write("          "); }
                        }
                    }
                    sWrtr.WriteLine("}),");
                }
            }
            sWrtr.WriteLine("    };");
            // ==========================================================================
            sWrtr.WriteLine();
            sWrtr.WriteLine("int NextState() {");
            sWrtr.WriteLine("    if (code == ScanBuff.EndOfFile)");
            sWrtr.WriteLine("        return eofNum;");
            sWrtr.WriteLine("    else");
            sWrtr.WriteLine("        unchecked {");
            sWrtr.WriteLine("            int rslt;");
            if (myTask.ChrClasses)
            {
                // If ChrClasses then either /unicode or explicit /classes option
                sWrtr.WriteLine("            int idx = {0} - NxS[state].min;", symStr);
                sWrtr.WriteLine("            if (idx < 0) idx += {0};", MaxSym);
            }
            else
            {
                // If NOT ChrClasses then this must be byte mode.
                sWrtr.WriteLine("            int idx = (byte)({0} - NxS[state].min);", symStr);
            }
            sWrtr.WriteLine("            if ((uint)idx >= (uint)NxS[state].rng) rslt = NxS[state].dflt;");
            sWrtr.WriteLine("            else rslt = NxS[state].nxt[idx];");
            sWrtr.WriteLine("            return rslt;");
            sWrtr.WriteLine("        }");
            sWrtr.WriteLine('}');
            // ==========================================================================
            if (myTask.aast.HasPredicates)
            {
                sWrtr.WriteLine();
                sWrtr.WriteLine("// EXPERIMENTAL: This is the NextState method that");
                sWrtr.WriteLine("// is used by the CharClassPredicate functions for V0.9");
                sWrtr.WriteLine("static int TestNextState(int qStat, int code) {");
                sWrtr.WriteLine("    if (code == ScanBuff.EndOfFile)");
                sWrtr.WriteLine("        return eofNum;");
                sWrtr.WriteLine("    else");
                sWrtr.WriteLine("        unchecked {");
                sWrtr.WriteLine("            int rslt;");
                if (myTask.ChrClasses)
                {
                    sWrtr.WriteLine("            int idx = {0} - NxS[qStat].min;", symStr);
                    sWrtr.WriteLine("            if (idx < 0) idx += {0};", MaxSym);
                }
                else // default alphabet is 256 wide, so cast to UINT8
                {
                    sWrtr.WriteLine("            int idx = (byte)({0} - NxS[qStat].min);", symStr);
                }
                sWrtr.WriteLine("            if ((uint)idx >= (uint)NxS[qStat].rng) rslt = NxS[qStat].dflt;");
                sWrtr.WriteLine("            else rslt = NxS[qStat].nxt[idx];");
                sWrtr.WriteLine("            return rslt;");
                sWrtr.WriteLine("        }");
                sWrtr.WriteLine('}');
            }
            // ==========================================================================
            sWrtr.WriteLine();
            sWrtr.WriteLine("#endregion");
            sWrtr.Flush();
        }

        /// <summary>
        ///  Emit uncompressed nextstate tables.
        /// </summary>
        /// <param name="sWrtr">The output text writer</param>
        internal void EmitRawTables(TextWriter sWrtr)
        {
            // bool isByte = stateList.Count < 128;
            bool doMap = myTask.ChrClasses;
            bool bigMap = doMap && myTask.CompressMap;
            var aliasPairs = new List<int>();
            string eType; //  = (isByte ? "sbyte" : "short");
            string symStr;

            if (stateList.Count < 128) eType = "sbyte";
            else if (stateList.Count < 0x8000) eType = "short";
            else eType = "int";

            if (bigMap && !myTask.Squeeze)
                useTwoLevelMap = (int)Math.Ceiling(Math.Log(myTask.partition.mapRuns.Count, 2)) > 3;

            if (!doMap)
                symStr = "code";
            else if (useTwoLevelMap)
                symStr = "Map(code)";
            else if (bigMap)
                symStr = "MapC(code)";
            else
                symStr = "map[code]";

            sWrtr.WriteLine("#region ScannerTables");
            sWrtr.Write("  static int[] startState = new int[] {");
            for (int i = 0; i < dfas.Length; i++)
            {
                DfsaInstance dInst = dfas[i];
                sWrtr.Write((dInst == null ? eofNum : dInst.start.Num));
                if (i < dfas.Length - 1)
                {
                    sWrtr.Write(", ");
                    if (i % 16 == 5) { sWrtr.WriteLine(); sWrtr.Write("        "); }
                }
            }
            sWrtr.WriteLine("};"); sWrtr.WriteLine();
            if (hasLeftAnchors)
            {
                sWrtr.Write("    static int[] anchorState = new int[] {");
                for (int i = 0; i < dfas.Length; i++)
                {
                    DfsaInstance dInst = dfas[i];
                    int anchorOrd = eofNum;
                    if (dInst != null)
                        anchorOrd = (dInst.anchor == null ? dInst.start.Num : dInst.anchor.Num);
                    sWrtr.Write((dInst == null ? eofNum : anchorOrd));
                    if (i < dfas.Length - 1)
                    {
                        sWrtr.Write(", ");
                        if (i % 16 == 5) { sWrtr.WriteLine(); sWrtr.Write("        "); }
                    }
                }
                sWrtr.WriteLine("};"); sWrtr.WriteLine();
            }
            if (bigMap)
                WriteCompressedMap2(sWrtr);
            else if (myTask.ChrClasses)
                WriteClassMap(sWrtr);

            int len = this.maxSym;
            // =================================================================
            sWrtr.WriteLine("  static {0}[][] nextState = new {0}[{1}][] {{", eType, stateList.Count);
            for (int i = 0; i < stateList.Count; i++)
            {
                DState dSt = stateList[i];
                // ====== Replace redundant rows with an alias =======
                bool usedShortCircuit = false;
                sWrtr.Write("/* nextState[{0, 4:D}] */ ", i);
                for (int j = 0; j < i; j++)
                    if (dSt.EquivalentNextStates(stateList[j]))
                    {
                        copyNum++;  // Keep count on alias rows for statistics
                        usedShortCircuit = true;
                        //  Cannot have a self-reference within an array initializer.
                        //  CSC does not complain, but the class constructor throws.
                        //  So... the following does not work ...
                        //          sWrtr.Write("nextState[{0}],", j);
                        aliasPairs.Add(i);
                        aliasPairs.Add(j);
                        sWrtr.Write("null,");
                        if (myTask.Verbose && foundShortestStrings)
                            sWrtr.Write(" // Shortest string \"{0}\"{1}",
                                CharacterUtilities.Map(dSt.shortestStr), Environment.NewLine);
                        else
                            sWrtr.WriteLine();
                        break;
                    }
                if (!usedShortCircuit)
                {
                    sWrtr.Write("new {0}[] {{", eType);
                    if (myTask.Verbose && foundShortestStrings) sWrtr.Write(
                        " // Shortest string \"{0}\"{1}        ",
                        CharacterUtilities.Map(dSt.shortestStr), Environment.NewLine);
                    for (int j = 0; j < len; j++)
                    {
                        if (dSt.GetNext(j) == null) sWrtr.Write(dSt.DefaultNext);
                        else sWrtr.Write(dSt.GetNext(j).Num);
                        if (j < (len - 1))
                        {
                            sWrtr.Write(", ");
                            if (j % 16 == 15) { sWrtr.WriteLine(); sWrtr.Write("        "); }
                        }
                    }
                    sWrtr.WriteLine("},");
                }
            }
            sWrtr.WriteLine("};");
            // Now we must fill in the missing row aliases.
            if (aliasPairs.Any())
            {
                sWrtr.WriteLine();
                sWrtr.WriteLine("  static {0}() {{", myTask.aast.scannerTypeName);
                for (int i = 0; i < aliasPairs.Count;)
                {
                    int leftIndex = aliasPairs[i++];
                    int rightIndex = aliasPairs[i++];
                    sWrtr.WriteLine("      nextState[{0}] = nextState[{1}];", leftIndex, rightIndex);
                }
                sWrtr.WriteLine("  }");
            }
            sWrtr.WriteLine();
            // =================================================================
            tranNum = (globNext - copyNum) * this.MaxSym;
            sWrtr.WriteLine();
            sWrtr.WriteLine("int NextState() {");
            sWrtr.WriteLine("    if (code == ScanBuff.EndOfFile)");
            sWrtr.WriteLine("        return eofNum;");
            sWrtr.WriteLine("    else");
            sWrtr.WriteLine("        return nextState[state][{0}];", symStr);
            sWrtr.WriteLine('}');
            // =================================================================
            if (myTask.aast.HasPredicates)
            {
                sWrtr.WriteLine();
                sWrtr.WriteLine("// EXPERIMENTAL: This is the NextState method that");
                sWrtr.WriteLine("// is used by the CharClassPredicate functions for V0.9");
                sWrtr.WriteLine("static int TestNextState(int query, int code) {");
                sWrtr.WriteLine("    if (code == ScanBuff.EndOfFile)");
                sWrtr.WriteLine("        return eofNum;");
                sWrtr.WriteLine("    else");
                sWrtr.WriteLine("        return nextState[query][{0}];", symStr);
                sWrtr.WriteLine('}');
            }
            // =================================================================
            sWrtr.WriteLine();
            sWrtr.WriteLine("#endregion");


            sWrtr.Flush();
        }
    }
}
