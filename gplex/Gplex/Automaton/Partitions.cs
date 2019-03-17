// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;
using System.Collections.Generic;
using System.Linq;
using QUT.Gplex.Automaton;
using QUT.Gplib;

namespace QUT.Gplex.Parser
{
    /// <summary>
    /// Partition is the class that represents a 
    /// partitioning of the character set into 
    /// equivalence classes for a particular set
    /// of literals from a given LEX specification.
    /// The literals are of two kinds: singleton
    /// characters and character sets [...].
    /// A partition object is initialized with 
    /// a single range from CharMin to CharMax,
    /// this is refined by the Refine method.
    /// The invariants of the partition are:
    /// (1) the character sets denoted by 
    /// the partition elements are disjoint,
    /// and together cover the character range.
    /// (2) for every processed literal L, and
    /// every partition element E, either every
    /// character in E is in L, or none is.
    /// </summary>
    internal class Partition
    {
        internal const int CutOff = 128; // Shortest run to consider
        internal const int PageSize = 256; // Pagesize for two-level map

        internal IList<PartitionElement> elements = new List<PartitionElement>();

        internal IList<MapRun> mapRuns;
        internal IList<MapRun> runsInBMP = new List<MapRun>();
        internal IList<MapRun> runsInNonBMPs = new List<MapRun>();
        internal TaskState myTask;

        internal IList<MapRun> pages;

        int[] iMap;
        CharClassMap tMap;

        /// <summary>
        /// Create a new partition
        /// </summary>
        /// <param name="symCard">The symbol alphabet cardinality</param>
        internal Partition(int symCard, TaskState task)
        {
            this.myTask = task;
            CharRange.Init(symCard);
            PartitionElement.Reset();

            elements.Add(PartitionElement.AllChars());
        }

        /// <summary>
        /// The mapping function from character
        /// ordinal to equivalence class ordinal.
        /// </summary>
        // internal int this[char ch] { get { return cMap[(int)ch]; } }
        internal int this[int chVal] { get { return tMap[chVal]; } }

        /// <summary>
        /// A projection of the inverse map from class ordinal
        /// back to character ordinal. iMap returns an example 
        /// character, used for diagnostic information only.
        /// </summary>
        internal int InvMap(int ch) { return iMap[ch]; }

        /// <summary>
        /// The number of equivalence classes.
        /// </summary>
        internal int Length { get { return elements.Count; } }

        internal void FindClasses(AAST aast)
        {
            var visitor = new Accumulator(this);
            foreach (RuleDesc rule in aast.ruleList)
            {
                rule.Tree.Visit(visitor);
            }
        }

        /// <summary>
        /// Fix the partition, and generate the forward and
        /// inverse mappings "char : equivalence class"
        /// </summary>
        internal void FixMap()
        {
            // Create the inverse map from partition element
            // ordinal to (an example) character.
            iMap = new int[elements.Count];
            foreach (PartitionElement pElem in elements)
            {
                if (pElem.list.Ranges.Any())
                    iMap[pElem.ord] = pElem.list.Ranges[0].minChr;
            }

            tMap = new CharClassMap(this);
            FindMapRuns();
        }

        /// <summary>
        /// Refine the current partition with respect 
        /// to the given range literal "lit".
        /// </summary>
        /// <param name="lit">The Range Literal</param>
        internal void Refine(RangeLiteral lit)
        {
            int idx;
            int max = elements.Count; // because this varies inside the loop
            //
            // For each of the *current* elements of the partition do:
            //
            for (idx = 0; idx < max; idx++)
            {
                PartitionElement elem = elements[idx];
                RangeList intersection = lit.list.AND(elem.list);
                // 
                // There are four cases here:
                // (1) No intersection of lit and elem ... do nothing
                // (2) Literal properly contains the partition element ...
                //     Add this element to the equivClasses list of this lit.
                //     Add this lit to the list of literals dependent on "elem".
                //     The intersection of other parts of the literal with other
                //     elements will be processed by other iterations of the loop.
                // (3) Literal is properly contained in the partition element ...
                //     Split the element into two: a new element containing the
                //     intersection, and an updated "elem" with the intersection
                //     subtracted. All literals dependent on the old element are
                //     now dependent on the new element and (the new version of)
                //     this element. The literal cannot overlap with any other 
                //     element, so the iteration can be terminated.
                // (4) Literal partially overlaps the partition element ...
                //     Split the element as for case 2.  Overlaps of the rest
                //     of the literal with other elements will be dealt with by
                //     other iterations of the loop. 
                //
                if (!intersection.IsEmpty) // not empty intersection
                {
                    // Test if elem is properly contained in lit
                    // If so, intersection == elem ...
                    if (intersection.EQU(elem.list))
                    {
                        elem.literals.Add(lit);
                        lit.equivClasses.Add(elem.ord);
                    }
                    else
                    {
                        PartitionElement newElem =
                            new PartitionElement(intersection.Ranges, false);
                        elements.Add(newElem);
                        lit.equivClasses.Add(newElem.ord);
                        newElem.literals.Add(lit);
                        //
                        //  We are about to split elem.
                        //  All literals that include elem
                        //  must now also include newElem
                        //
                        foreach (RangeLiteral rngLit in elem.literals)
                        {
                            rngLit.equivClasses.Add(newElem.ord);
                            newElem.literals.Add(rngLit);
                        }
                        elem.list = elem.list.SUB(intersection);
                        //
                        // Test if lit is a subset of elem
                        // If so, intersection == lit and we can
                        // assert that no other loop iteration has
                        // a non-empty intersection with this lit.
                        //
                        if (intersection.EQU(lit.list))
                            return;
                    }
                }
            }
        }

        internal void FindMapRuns()
        {
            List<MapRun> result = new List<MapRun>();
            int cardinality = CharRange.SymCard;
            int start = 0;                       // Start of the run
            int finish = 0;
            // Get first element and add to List
            tMap.GetEnclosingRange(start, out start, out finish);
            result.Add(new MapRun(start, finish));
            start = finish + 1;
            // Now get all the rest ...
            while (start < cardinality)
            {
                tMap.GetEnclosingRange(start, out start, out finish);
                MapRun lastRun = result[result.Count - 1];
                int length = finish - start + 1;
                if (length >= Partition.CutOff || lastRun.tag == MapRun.TagType.longRun)
                    result.Add(new MapRun(start, finish));
                else
                    lastRun.Merge(start, finish);
                start = finish + 1;
            }
            int total = 0;
            foreach (MapRun run in result)
            {
                if (run.tag == MapRun.TagType.mixedValues)
                    total += run.Length;
                if (run.range.maxChr <= Char.MaxValue) // ==> run is BMP
                    runsInBMP.Add(run);
                else if (run.range.minChr > Char.MaxValue) // ==> not in BMP
                    runsInNonBMPs.Add(run);
                else
                {
                    MapRun lowPart = new MapRun(run.range.minChr, Char.MaxValue);
                    MapRun highPart = new MapRun(Char.MaxValue + 1, run.range.maxChr);
                    if (run.range.minChr != Char.MaxValue)
                        lowPart.tag = run.tag;
                    if (Char.MaxValue + 1 != run.range.maxChr)
                        highPart.tag = run.tag;
                    runsInBMP.Add(lowPart);
                    runsInNonBMPs.Add(highPart);
                }
            }
            this.mapRuns = result;
        }

        internal void ComputePages()
        {
            var result = new List<MapRun>();
            // int cardinality = CharRange.SymCard;
            int cardinality = Char.MaxValue + 1;
            int stepSize = PageSize;
            int current = 0;                       // Start of the run
            while (current < cardinality)
            {
                int start, finish;
                int limit = current + stepSize - 1;
                MapRun thisRun;
                tMap.GetEnclosingRange(current, out start, out finish);
                if (finish > limit) finish = limit;
                thisRun = new MapRun(current, finish, tMap[current]);
                result.Add(thisRun);
                current = finish + 1;
                while (current <= limit)
                {
                    tMap.GetEnclosingRange(current, out start, out finish);
                    if (finish > limit) finish = limit;
                    thisRun.Merge(current, finish, tMap[current]);
                    current = finish + 1;
                }
            }
            this.pages = result;
        }

        /// <summary>
        /// Compares two map runs, but only for the cheap case
        /// of maps which are NOT mixed values.
        /// </summary>
        /// <param name="lRun">The LHS run</param>
        /// <param name="rRun">The RHS run</param>
        /// <returns>True if equal</returns>
        internal bool EqualMaps(MapRun lRun, MapRun rRun)
        {
            return
                (lRun.tag != MapRun.TagType.mixedValues &&
                rRun.tag != MapRun.TagType.mixedValues &&
                tMap[lRun.range.minChr] == tMap[rRun.range.minChr] &&
                tMap[lRun.range.maxChr] == tMap[rRun.range.maxChr]);
        }
    }
}
