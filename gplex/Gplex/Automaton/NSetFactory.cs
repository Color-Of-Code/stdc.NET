// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


namespace QUT.Gplex.Automaton
{
    internal partial class DFSA
    {
        /// <summary>
        /// This class is a factory for the objects that
        /// represent sets of NFSA states.  The sets are arrays 
        /// of bit sets mapped onto a uint32 array.  The length
        /// of the arrays is frozen at the time that the factory
        /// is instantiated, as |NFSA| div 32
        /// </summary>
        internal partial class NSetFactory
        {
            private int length;
            public NSetFactory(int nfsaCardinality) { length = (nfsaCardinality + 31) / 32; }
            public NSet MkNewSet() { return new NSet(length); }
        }
    }
}
