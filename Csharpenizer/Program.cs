﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Csharpenizer {
	class Program {

		private static String[] CFunctions = new String[] {
			"fclose",
			"fopen",
			"fprintf",
			"fputc",
			"fputs",
			"gets",
			"printf",
			"puts",
			"qsort",
			"rand",
			"scanf",
			"srand",
			"strcat",
			"strcpy",
			"time",
			"NULL",
		};

		static void Main (string[] args)
		{
			string input = @"G:\tests\dotNet_Tests\Projekte\Projekte\stdc\Csharpenizer\test\in";
			string output = @"G:\tests\dotNet_Tests\Projekte\Projekte\stdc\Csharpenizer\test\out";

			DirectoryInfo din = new DirectoryInfo (input);
			DirectoryInfo dout = new DirectoryInfo (output);
			foreach (FileInfo fi in din.GetFiles ()) {
				ProcessFile (fi, dout);
			}
		}

		private static void ProcessFile (FileInfo fi, DirectoryInfo dout)
		{
			try {
				if (fi.Name.EndsWith (".c", StringComparison.InvariantCultureIgnoreCase) ||
					fi.Name.EndsWith (".h", StringComparison.InvariantCultureIgnoreCase)) {
					FileInfo fout = new FileInfo (Path.Combine (dout.FullName, fi.Name.Replace (".", "_") + ".cs"));
					ProcessFile (fi, fout);
				}
			}
			catch (Exception ex) {
				Console.Error.WriteLine(String.Format("{0}: {1}", fi.Name, ex.Message));
			}
		}

		private static void ProcessFile (FileInfo fi, FileInfo fo)
		{
			StringBuilder sb = new StringBuilder ();

			Prefix (sb);

			string text = File.ReadAllText (fi.FullName);

			CLexer.Scanner scanner = new CLexer.Scanner ();
			scanner.SetSource (text, 0);
			scanner.yylex ();

			//string newtext = text;
			//newtext = Regex.Replace (newtext, @"(#\s*include\s)", "// $1");
			//newtext = Regex.Replace (newtext, @"\bFILE\s*\*\s+", "C.FILE ");
			//newtext = Regex.Replace (newtext, @"\bconst\s+void\s*\*\s*", "object ");
			//newtext = Regex.Replace (newtext, @"\bvoid\s*\*\s*", "object ");
			//newtext = Regex.Replace (newtext, @"&(\w+)", "out $1");
			//newtext = Regex.Replace (newtext, @"int\s+(\w+)\s*\[\s*\]", "int[] $1");
			//newtext = Regex.Replace (newtext, @"char\s+(\w+)\s*\[(\d+)\]\s*;", "char[] $1 = new char[$2];");
			//newtext = Regex.Replace (newtext, @"(const\s+)?char\s+(\w+)\s*\[\s*\]\s*=", "string $2 =");
			//foreach (string func in CFunctions)
			//    newtext = Regex.Replace (newtext, String.Format (@"\b({0})\b", func), "C.$1");

			//sb.AppendLine (newtext);
			////foreach (string line in File.ReadAllLines (fi.FullName)) {
			////    string newline = line;
			////    if (line.Contains ("#include")) {
			////        newline = String.Format ("// {0}", line);
			////    }
			////    sb.AppendLine (newline);
			////}
			sb.Append (scanner.Text);
			sb.AppendLine ();

			Postfix (scanner.ContainsIMain, scanner.ContainsVMain, sb);
			File.WriteAllText (fo.FullName, sb.ToString ());
		}

		private static void Prefix (StringBuilder sb)
		{
			sb.AppendLine ("// generated by Csharpenizer (http://www.color-of-code.de)");
			sb.AppendLine ("#region Namespace wrapping");
			sb.AppendLine ("//css_reference stdc.dll;");
			sb.AppendLine ("using stdc;");
			sb.AppendLine ();
			sb.AppendLine ("namespace main {");
			sb.AppendLine ("  public partial class Program {");
			sb.AppendLine ("#endregion");
			sb.AppendLine ();
		}

		private static void Postfix (Boolean containsIMain, Boolean containsVMain, StringBuilder sb)
		{
			sb.AppendLine ();
			if (containsVMain || containsIMain) {
				sb.AppendLine ("    #region Main trampoline");
				sb.AppendLine ("    static int Main (string[] args) {");
				sb.AppendLine ("      Program p = new Program();");
				if (containsVMain) {
					sb.AppendLine ("      C.RunVMain (args, p.main);");
					sb.AppendLine ("      return 0;");
				} else {
					sb.AppendLine ("      return C.RunIMain (args, p.main);");
				}
				sb.AppendLine ("    }");
				sb.AppendLine ("    #endregion");
			}

			sb.AppendLine ("#region Namespace wrapping");
			sb.AppendLine ("  }");
			sb.AppendLine ("}");
			sb.AppendLine ("#endregion");
		}
	}
}
