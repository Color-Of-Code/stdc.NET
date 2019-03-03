// Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, K John Gough, QUT 2006-2010
// (see accompanying GPPGcopyright.rtf)
// This file author: John Gough

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace QUT.GPGen
{
    public delegate TOut Mapper<TOut, TIn>(TIn input);

    public static class ListUtilities
    {
        public const int LineLength = 80;

        public static string GetStringFromList<T>(IEnumerable<T> list)
        {
            return GetStringFromList<T>(list, ", ", 4, true);
        }

        public static string GetStringFromList<T>(IEnumerable<T> list, string separator, int indent)
        {
            return GetStringFromList<T>(list, separator, indent, true);
        }

        public static string GetStringFromList<T>(IEnumerable<T> list, string separator, int indent, bool lineBreak)
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

        //public static List<TOut> Map<TOut, TIn>(List<TIn> input, Mapper<TOut, TIn> map)
        //{
        //    List<TOut> rslt = new List<TOut>(input.Count);
        //    foreach (TIn elem in input)
        //        rslt.Add(map(elem));
        //    return rslt;
        //}

        public static Collection<TOut> MapC<TOut, TIn>(IEnumerable<TIn> input, Mapper<TOut, TIn> map)
        {
            Collection<TOut> rslt = new Collection<TOut>();
            foreach (TIn elem in input)
                rslt.Add(map(elem));
            return rslt;
        }


    }
}