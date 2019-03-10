// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, K John Gough, QUT 2006-2010
// (see accompanying GPPGcopyright.rtf)
// This file author: John Gough

using System;
using System.Collections.Generic;

namespace QUT.GPGen
{
    public delegate TOut Mapper<TOut, TIn>(TIn input);

    public static class ListUtilities
    {
        public const int LineLength = 80;

        public static string GetStringFromList<T>(IEnumerable<T> list,
            string separator = ", ",
            int indent = 4,
            bool lineBreak = true)
        {
            int lastBreak = -indent;
            bool more = false;
            string indentStr = new String(' ', indent);
            string listBreak = System.Environment.NewLine + indentStr;
            var builder = new System.Text.StringBuilder();

            var e = list.GetEnumerator();
            if (e.MoveNext())
                do
                {
                    T nt = e.Current;
                    string addend = nt.ToString();
                    if (lineBreak && builder.Length + addend.Length >= lastBreak + LineLength)
                    {
                        lastBreak = builder.Length;
                        builder.Append(listBreak);
                    }
                    more = e.MoveNext();
                    builder.AppendFormat("{0}{1}", nt.ToString(), (more ? separator : ""));
                } while (more);

            return builder.ToString();
        }
    }
}
