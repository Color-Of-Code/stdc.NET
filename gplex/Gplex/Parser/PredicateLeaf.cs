// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System;
using System.Text;

namespace QUT.Gplex.Parser
{
    #region AST for Regular Expressions

    internal sealed class PredicateLeaf : Leaf
    {
        CharTest Test;

        internal PredicateLeaf() : base(RegOp.charClass) { }

        internal PredicateLeaf(CharTest test)
            : base(RegOp.charClass) { this.Test = test; }

        internal static CharTest MkCharTest(CharPredicate cPred, CodePointPredicate cpPred)
        {
            return delegate (int ord)
            {
                // Use the Char function for the BMP
                if (ord <= (int)Char.MaxValue)
                    return cPred((char)ord);
                else
                    return cpPred(Char.ConvertFromUtf32(ord), 0);
            };
        }

        /// <summary>
        /// This method constructs a RangeLiteral holding
        /// all of the codepoints from all planes for which
        /// the Test delegate returns true.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="aast"></param>
        /// <param name="max"></param>
        internal void Populate(string name, AAST aast)
        {
            DateTime begin = DateTime.Now;

            int max = aast.Task.TargetSymCardinality;

            this.rangeLit = new RangeLiteral(false);
            //
            // Run the delegate over all the values 
            // between '\0' and (max-1).  Find contiguous
            // true values and add to the CharRange list.
            //
            int j = 0;
            int codepage = aast.CodePage;
            if (max > 256 ||
                codepage == Automaton.TaskState.rawCP ||
                codepage == Automaton.TaskState.guessCP)
            {
                if (max <= 256 && codepage == Automaton.TaskState.guessCP)
                    aast.hdlr.ListError(aast.AtStart, 93);
                //
                // We are generating a set of numeric code points with
                // the named property.  No interpretation is needed, either
                // (1) because this is for a unicode scanner that has
                //     already decoded its input element stream, OR
                // (2) the user has commanded /codepoint:raw to indicate
                //     that no interpretation is to be used.
                //
                while (j < max)
                {
                    int start;
                    while (j < max && !Test(j))
                        j++;
                    if (j == max)
                        break;
                    start = j;
                    while (j < max && Test(j))
                        j++;
                    this.rangeLit.list.Add(new CharRange(start, (j - 1)));
                }
            }
            else
            {
                // We are generating a set of byte values from the
                // 0x00 to 0xFF "alphabet" that correspond to unicode
                // characters with the named property.  The meaning of
                // "corresponds" is defined by the nominated codepage.
                //
                // Check codepage for single byte property.
                //
                Encoding enc = Encoding.GetEncoding(codepage);
                Decoder decoder = enc.GetDecoder();
                if (!enc.IsSingleByte)
                    aast.hdlr.ListError(aast.AtStart, 92);
                //
                // Construct character map for bytes.
                //
                int bNum, cNum;
                bool done;
                char[] cArray = new char[256];
                byte[] bArray = new byte[256];
                for (int b = 0; b < 256; b++)
                {
                    bArray[b] = (byte)b;
                    cArray[b] = '?';
                }
                decoder.Convert(bArray, 0, 256, cArray, 0, 256, true, out bNum, out cNum, out done);
                //
                // Now construct the CharRange literal
                //
                while (j < max)
                {
                    int start;
                    while (j < max && !Test(cArray[j]))
                        j++;
                    if (j == max)
                        break;
                    start = j;
                    while (j < max && Test(cArray[j]))
                        j++;
                    this.rangeLit.list.Add(new CharRange(start, (j - 1)));
                }
            }
            if (aast.IsVerbose)
            {
                Console.WriteLine("GPLEX: Generating [:{0}:], {1}", name, Gplex.Automaton.TaskState.ElapsedTime(begin));
            }
        }
    }
    #endregion
}
