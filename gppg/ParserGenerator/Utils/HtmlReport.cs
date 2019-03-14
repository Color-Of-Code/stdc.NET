
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using QUT.Gplib;

namespace QUT.GPGen
{
    public class HtmlReport
    {
        public HtmlReport()
        {
        }
        internal void GenerateReport(StreamWriter writer, string inputFilename,
            IList<AutomatonState> statelist,
            Grammar grammar)
        {
            writer.WriteLine("<b>Grammar {0}</b>", inputFilename);
            WriteProductions(writer, grammar.productions);

            foreach (AutomatonState state in statelist)
            {
                writer.WriteLine(StateAnchor(state.num));
                writer.WriteLine(StateToString(state));
            }
        }

        internal void GenerateCompoundReport(StreamWriter writer, string inputFilename,
            IList<AutomatonState> statelist,
            Grammar grammar)
        {
            writer.WriteLine("<b>Grammar and Diagnosis {0}</b>", inputFilename);

            WriteProductions(writer, grammar.productions);
            DiagnosticHelp.PopulatePrefixes(statelist);
            Mapper<string, AutomatonState> map = state => StateRef(state.num);

            foreach (AutomatonState state in statelist)
            {
                writer.WriteLine(StateAnchor(state.num));
                DiagnoseState(writer, state, map, false);
                writer.WriteLine(StateToString(state));
            }
        }


        static void DiagnoseState<T>(StreamWriter writer, AutomatonState state, Mapper<T, AutomatonState> map, bool doKernel)
        {
            IEnumerable<T> statePath = state.statePath.Select(x => map(x));

            writer.WriteLine("    Shortest prefix: {0}", ListUtilities.GetStringFromList(state.shortestPrefix, " ", 8));
            writer.WriteLine("    State path: {0}", ListUtilities.GetStringFromList(statePath, "->", 8, false));
            if (state.conflicts != null)
            {
                writer.WriteLine();
                writer.WriteLine("    <b>Conflicts in this state</b>");
                foreach (Conflict conflict in state.conflicts)
                {
                    conflict.HtmlReport(writer);
                }
            }
            if (doKernel)
            {
                writer.WriteLine("    Kernel items --");
                foreach (ProductionItem item in state.kernelItems)
                    writer.WriteLine("      {0}", ItemToString(item, false));
            }
            writer.WriteLine();
        }

        static string StateToString(AutomatonState thisState)
        {
            var builder = new StringBuilder();

            builder.AppendLine(Header2("Kernel Items"));
            foreach (ProductionItem item in thisState.kernelItems)
            {
                builder.AppendFormat("    {0}", ItemToString(item, true));
                builder.AppendLine();
            }

            builder.AppendLine();

            if (thisState.parseTable.Any())
                builder.AppendLine(Header2("Parser Actions"));
            foreach (var a in thisState.parseTable)
            {
                builder.AppendFormat("    {0,-14} {1}", a.Key, ActionToString(a.Value));
                builder.AppendLine();
            }

            builder.AppendLine();

            if (thisState.nonTerminalTransitions.Any())
                builder.AppendLine(Header2("Transitions"));
            foreach (var n in thisState.nonTerminalTransitions)
            {
                builder.AppendFormat("    {0,-14} go to state {1}", n.Key, StateRef(thisState.Goto[n.Key].num));
                builder.AppendLine();
            }

            builder.AppendLine();

            return builder.ToString();
        }

        static string ActionToString(IParserAction action)
        {
            string result = null;
            Shift shift = action as Shift;
            if (shift != null)
                return "shift, and go to state " + StateRef(shift.next.num);
            Reduce reduce = action as Reduce;
            if (reduce != null)
                return String.Format(CultureInfo.InvariantCulture, "reduce using {0} ({1}{2})",
                    ProductionRef(reduce.item.production.num),
                    (reduce.item.production.rhs.Count == 0 ? "Erasing " : ""),
                    reduce.item.production.lhs);
            return result;
        }

        static string ItemToString(ProductionItem item, bool doLookAhead)
        {
            int lhsLength;
            var list = new List<string>();
            var builder = new StringBuilder();

            builder.AppendFormat("{0} {1}: ", item.production.num, item.production.lhs);
            lhsLength = builder.Length;

            for (int i = 0; i < item.production.rhs.Count; i++)
            {
                if (i == item.pos)
                    list.Add(".");
                list.Add(item.production.rhs[i].ToString());
            }

            if (item.pos == item.production.rhs.Count)
                list.Add(".");

            builder.Append(ListUtilities.GetStringFromList(list, " ", lhsLength + 6));

            if (item.LookAhead != null && doLookAhead)
            {
                builder.AppendLine();
                builder.AppendFormat("\t-lookahead: {{ {0} }}", ListUtilities.GetStringFromList(item.LookAhead, ", ", 16));
            }

            return builder.ToString();
        }

        void WriteProductions(StreamWriter writer, IEnumerable<Production> productions)
        {
            NonTerminal lhs = null;

            foreach (Production production in productions)
            {
                int lhsLength = production.lhs.ToString().Length;

                if (production.lhs != lhs)
                {
                    lhs = production.lhs as NonTerminal;  // HACK: TODO: fix
                    writer.WriteLine();
                    writer.Write("{0} {1}: ", ProductionAnchor(production.num), lhs);
                }
                else
                    writer.Write("{0} {1}| ", ProductionAnchor(production.num), new string(' ', lhsLength));

                if (production.rhs.Count == 0)
                    writer.WriteLine("/* empty */");
                else
                    writer.WriteLine(ListUtilities.GetStringFromList(production.rhs, " ", lhsLength + 12));
            }

            writer.WriteLine();
        }

        internal void HtmlHeader(StreamWriter wrtr, string fileinfo)
        {
            wrtr.WriteLine("<html><head><title>{0}</title></head>", fileinfo);
            wrtr.WriteLine("<body bgcolor=\"white\">");
            wrtr.WriteLine("<hr><pre>");
        }

        internal void HtmlTrailer(StreamWriter wrtr)
        {
            //wrtr.WriteLine("</font></pre></hr></body></html>");
            wrtr.WriteLine("</pre></hr></body></html>");
        }
        static string ProductionAnchor(int prodNum)
        {
            return String.Format(CultureInfo.InvariantCulture, "<a name=\"prod{0}\">{0,5}</a>", prodNum);
        }

        static string ProductionRef(int prodNum)
        {
            return String.Format(CultureInfo.InvariantCulture, "<a href=\"#prod{0}\">rule {0}</a>", prodNum);
        }

        static string StateAnchor(int stateNum)
        {
            // return String.Format(CultureInfo.InvariantCulture, "<b><a name=\"state{0}\">State {0}</a></b>", stateNum);
            return String.Format(CultureInfo.InvariantCulture, "<b><a name=\"state{0}\">State</a> {1}</b>", stateNum, StateRef(stateNum));
        }

        static string StateRef(int stateNum)
        {
            return String.Format(CultureInfo.InvariantCulture, "<a href=\"#state{0}\">{0}</a>", stateNum);
        }

        static string Header2(string display)
        {
            return String.Format(CultureInfo.InvariantCulture, "  <b>{0}</b>", display);
        }

    }
}
