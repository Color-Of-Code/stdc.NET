// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)



namespace QUT.Gplex
{

    public interface ICharTestFactory
    {
        CharTest GetDelegate(string name);
    }

    /// <summary>
    /// Delegate type for predicates over code points.
    /// </summary>
    /// <param name="ord"></param>
    /// <returns></returns>
    public delegate bool CharTest(int ordinal);

    /// <summary>
    /// Delegate type for applying a predicate to a char value.
    /// Note that this only applies to code points in the 
    /// basic multilingual plane, i.e. chr &lt; Char.MaxValue
    /// </summary>
    /// <param name="chr"></param>
    /// <returns></returns>
    internal delegate bool CharPredicate(char chr);

    /// <summary>
    /// Delegate type for applying a predicate to code point.
    /// The code point might be represented by a surrogate 
    /// pair at the prescribed position in the string.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    internal delegate bool CodePointPredicate(string str, int index);
}
