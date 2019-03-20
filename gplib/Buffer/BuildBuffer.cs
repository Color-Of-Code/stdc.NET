using System.IO;
using System.Text;

namespace QUT.Gplib
{
    public delegate int BlockReader(char[] block, int index, int number);

    // A delegate factory, serving up a delegate that
    // reads a block of characters from the underlying
    // encoded stream, via a StreamReader object.
    //
    public static class BlockReaderFactory
    {
        public static BlockReader Raw(Stream stream)
        {
            return delegate(char[] block, int index, int number)
            {
                byte[] b = new byte[number];
                int count = stream.Read(b, 0, number);
                int i = 0;
                int j = index;
                for (; i < count; i++, j++)
                    block[j] = (char)b[i];
                return count;
            };
        }

        //#if (!BYTEMODE)
        public static BlockReader Get(Stream stream, int fallbackCodePage)
        {
            Encoding encoding;
            int preamble = Preamble(stream);

            if (preamble != 0)  // There is a valid BOM here!
                encoding = Encoding.GetEncoding(preamble);
            else if (fallbackCodePage == -1) // Fallback is "raw" bytes
                return Raw(stream);
            else if (fallbackCodePage != -2) // Anything but "guess"
                encoding = Encoding.GetEncoding(fallbackCodePage);
            else // This is the "guess" option
            {
                encoding = Encoding.UTF8;
                // TODO: migrate guesser
                /*
                int guess = new Guesser(stream).GuessCodePage();
                stream.Seek(0, SeekOrigin.Begin);
                if (guess == -1) // ==> this is a 7-bit file
                    encoding = Encoding.ASCII;
                else if (guess == 65001)
                    encoding = Encoding.UTF8;
                else             // ==> use the machine default
                    encoding = Encoding.Default;
                */
            }
            StreamReader reader = new StreamReader(stream, encoding);
            return reader.Read;
        }

        static int Preamble(Stream stream)
        {
            int b0 = stream.ReadByte();
            int b1 = stream.ReadByte();

            if (b0 == 0xfe && b1 == 0xff)
                return 1201; // UTF16BE
            if (b0 == 0xff && b1 == 0xfe)
                return 1200; // UTF16LE

            int b2 = stream.ReadByte();
            if (b0 == 0xef && b1 == 0xbb && b2 == 0xbf)
                return 65001; // UTF8
            //
            // There is no unicode preamble, so we
            // return denoter for the machine default.
            //
            stream.Seek(0, SeekOrigin.Begin);
            return 0;
        }
        // #endif // !BYTEMODE
    }
    // #endif // !NOFILES

    // #if (!NOFILES)
    // ==============================================================
    // =====     class BuildBuff : for unicode text files    ========
    // ==============================================================

    class BuildBuffer : ScanBuff
    {
        // Double buffer for char stream.
        class BufferElement
        {
            StringBuilder bldr = new StringBuilder();
            StringBuilder next = new StringBuilder();
            int minIx;
            int maxIx;
            int brkIx;
            bool appendToNext;

            internal BufferElement() { }

            internal int MaxIndex { get { return maxIx; } }
            // internal int MinIndex { get { return minIx; } }

            internal char this[int index]
            {
                get
                {
                    if (index < minIx || index >= maxIx)
                        throw new BufferException("Index was outside data buffer");
                    else if (index < brkIx)
                        return bldr[index - minIx];
                    else
                        return next[index - brkIx];
                }
            }

            internal void Append(char[] block, int count)
            {
                maxIx += count;
                if (appendToNext)
                    this.next.Append(block, 0, count);
                else
                {
                    this.bldr.Append(block, 0, count);
                    brkIx = maxIx;
                    appendToNext = true;
                }
            }

            internal string GetString(int start, int limit)
            {
                if (limit <= start)
                    return "";
                if (start >= minIx && limit <= maxIx)
                    if (limit < brkIx) // String entirely in bldr builder
                        return bldr.ToString(start - minIx, limit - start);
                    else if (start >= brkIx) // String entirely in next builder
                        return next.ToString(start - brkIx, limit - start);
                    else // Must do a string-concatenation
                        return
                            bldr.ToString(start - minIx, brkIx - start) +
                            next.ToString(0, limit - brkIx);
                else
                    throw new BufferException("String was outside data buffer");
            }

            internal void Mark(int limit)
            {
                if (limit > brkIx + 16) // Rotate blocks
                {
                    StringBuilder temp = bldr;
                    bldr = next;
                    next = temp;
                    next.Length = 0;
                    minIx = brkIx;
                    brkIx = maxIx;
                }
            }
        }

        BufferElement data = new BufferElement();

        int bPos;            // Postion index in the StringBuilder
        BlockReader NextBlk; // Delegate that serves char-arrays;

        private string EncodingName
        {
            get
            {
                StreamReader rdr = NextBlk.Target as StreamReader;
                return (rdr == null ? "raw-bytes" : rdr.CurrentEncoding.BodyName);
            }
        }

        public BuildBuffer(Stream stream)
        {
            FileStream fStrm = (stream as FileStream);
            if (fStrm != null) FileName = fStrm.Name;
            NextBlk = BlockReaderFactory.Raw(stream);
        }

        // #if (!BYTEMODE)
        public BuildBuffer(Stream stream, int fallbackCodePage)
        {
            FileStream fStrm = (stream as FileStream);
            if (fStrm != null) FileName = fStrm.Name;
            NextBlk = BlockReaderFactory.Get(stream, fallbackCodePage);
        }

        /// <summary>
        /// Marks a conservative lower bound for the buffer,
        /// allowing space to be reclaimed.  If an application 
        /// needs to call GetString at arbitrary past locations 
        /// in the input stream, Mark() is not called.
        /// </summary>
        public override void Mark() { data.Mark(bPos - 2); }

        public override int Pos
        {
            get { return bPos; }
            set { bPos = value; }
        }


        /// <summary>
        /// Read returns the ordinal number of the next char, or 
        /// EOF (-1) for an end of stream.  Note that the next
        /// code point may require *two* calls of Read().
        /// </summary>
        /// <returns></returns>
        public override int Read()
        {
            //
            //  Characters at positions 
            //  [data.offset, data.offset + data.bldr.Length)
            //  are available in data.bldr.
            //
            if (bPos < data.MaxIndex)
            {
                // ch0 cannot be EOF
                return (int)data[bPos++];
            }
            else // Read from underlying stream
            {
                // Experimental code, blocks of page size
                char[] chrs = new char[4096];
                int count = NextBlk(chrs, 0, 4096);
                if (count == 0)
                    return EndOfFile;
                else
                {
                    data.Append(chrs, count);
                    return (int)data[bPos++];
                }
            }
        }

        public override string GetString(int begin, int limit)
        {
            return data.GetString(begin, limit);
        }

        public override string ToString()
        {
            return "StringBuilder buffer, encoding: " + this.EncodingName;
        }
    }
}
