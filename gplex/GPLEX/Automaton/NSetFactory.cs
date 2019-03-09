// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;

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
        internal class NSetFactory
        {
            private int length;
            public NSetFactory(int nfsaCardinality) { length = (nfsaCardinality + 31) / 32; }
            public NSet MkNewSet() { return new NSet(length); }

            /// <summary>
            /// The sets themselves.  The class needs to implement
            /// IEquatable and override GetHashCode if it is to be
            /// used in a dictionary with Hashtable lookup 
            /// </summary>
            public class NSet : IEquatable<NSet>
            {
                private uint[] arr;
                internal NSet(int length) { arr = new uint[length]; }

                public bool Equals(NSet val)
                {
                    // Short-circuit the test if possible, as for string comparisons
                    for (int i = 0; i < arr.Length; i++) if (arr[i] != val.arr[i]) return false;
                    return true;
                }

                public override int GetHashCode()
                {
                    // The hash code is a word-wise XOR
                    uint val = arr[0];
                    for (int i = 1; i < arr.Length; i++) val ^= arr[i];
                    return (int)val;
                }

                public void Insert(int ord) { arr[ord / 32] |= (uint)(1 << ord % 32); }
                public bool Contains(int ord) { return (arr[ord / 32] & (uint)(1 << (ord % 32))) != 0; }
                public NEnum GetEnumerator() { return new NEnum(this.arr); }

                //public string Diag()
                //{
                //    string rslt = "";
                //    NEnum iter = this.GetEnumerator();
                //    while (iter.MoveNext())
                //    {
                //        int i = iter.Current;
                //        rslt += i.ToString();
                //        rslt += ",";
                //    }
                //    return rslt;
                //}
            }

            /// <summary>
            /// This is a custom enumerator.  It does not implement 
            /// IEnumerator, and so cannot be used in foreach statements.
            /// </summary>
            public class NEnum
            {
                uint[] arr;
                uint elem;      // having elem zero is essential both initially and after Reset.
                int current, idx, ord;
                internal NEnum(uint[] dat) { arr = dat; current = -1; idx = -1; }
                // public void Reset() { current = -1; idx = -1; elem = 0; }

                public int Current
                { get { if (current < 0) throw new InvalidOperationException(); else return current; } }

                public bool MoveNext()
                {
                    // The essence of the code is to skip quickly over runs
                    // of zeros in sparse sets. The code quickly skips over
                    // whole uint32 words that are empty, and skips over 
                    // 8-bit runs of zeros also.
                    // Post-condition: elem and ord denote the next position
                    //                 that might hold a set element.
                    //                 idx corresponds to the current elem.
                    while (true)
                    {
                        if (elem != 0)  // try to find non-zero bit - there is at least one!
                        {
                            while ((elem & 255) == 0) { elem /= 256; ord += 8; } // discard 8 at a time
                            while ((elem & 1) == 0) { elem /= 2; ord++; }        // now one at a time
                            current = idx * 32 + ord;                            // compute ordinal position
                            elem /= 2; ord++;                                    // establish post condition
                            return true;
                        }
                        else
                        {
                            idx++;                                               // get the next array index
                            if (idx >= arr.Length) return false;                 // check for array ended
                            elem = arr[idx];                                     // else get the next element
                            ord = 0;                                             // establish post condition
                        }
                    }
                }
            }
        }
    }
}
