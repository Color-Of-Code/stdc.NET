// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;

namespace QUT.Gplex.Automaton
{
    /// <summary>
    /// This is a custom enumerator.  It does not implement 
    /// IEnumerator, and so cannot be used in foreach statements.
    /// </summary>
    public class NEnumerator
    {
        uint[] arr;
        uint elem;      // having elem zero is essential both initially and after Reset.
        int current, idx, ord;
        internal NEnumerator(uint[] dat)
        {
            arr = dat;
            current = -1;
            idx = -1;
        }
        // public void Reset() { current = -1; idx = -1; elem = 0; }

        public int Current
        {
            get
            {
                if (current < 0)
                    throw new InvalidOperationException(); else return current;
            }
        }

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
