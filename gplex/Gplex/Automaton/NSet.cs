// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)

using System;

namespace QUT.Gplex.Automaton
{
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
        public NEnumerator GetEnumerator()
        {
            return new NEnumerator(this.arr);
        }

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
}
