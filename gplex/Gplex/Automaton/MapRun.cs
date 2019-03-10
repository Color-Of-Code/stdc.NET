// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


namespace QUT.Gplex.Parser
{
    /// <summary>
    /// For the compression of sparse maps, adjacent sequences
    /// of character values are denoted as singletons (a single char),
    /// shortRuns (a run of identical values but shorter than CUTOFF)
    /// longRuns (a run of identical values longer than CUTOFF) and
    /// mixed values (a run containing two or more different values.
    /// </summary>
    internal class MapRun
    {
        internal enum TagType { empty, singleton, shortRun, longRun, mixedValues }

        internal TagType tag = TagType.empty;
        internal CharRange range;
        private int tableOrd = -1;
        private int hash;

        internal MapRun(int min, int max, int val)
        {
            hash = (max - min + 1) * val;
            range = new CharRange(min, max);
            if (min == max)
                tag = TagType.singleton;
            else if (max - min + 1 >= Partition.CutOff)
                tag = TagType.longRun;
            else
                tag = TagType.shortRun;
        }

        internal MapRun(int min, int max) : this(min, max, 0) { }

        internal int Length
        {
            get { return ((int)range.maxChr - (int)range.minChr + 1); }
        }

        internal int TableOrd { get { return tableOrd; } set { tableOrd = value; } }

        internal void Merge(int min, int max) { Merge(min, max, 0); }

        internal void Merge(int min, int max, int val)
        {
            if (this.range.maxChr != (min - 1))
                throw new GplexInternalException("Bad MapRun Merge");
            this.range.maxChr = max;
            this.tag = TagType.mixedValues;
            this.hash += (max - min + 1) * val;
        }
    }
}
