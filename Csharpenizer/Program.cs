using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharpenizer {
	class Program {
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
			if (fi.Name.EndsWith (".c", StringComparison.InvariantCultureIgnoreCase) ||
				fi.Name.EndsWith (".h", StringComparison.InvariantCultureIgnoreCase)) {
				FileInfo fout = new FileInfo (Path.Combine (dout.FullName, fi.Name.Replace (".", "_") + ".cs"));
				ProcessFile (fi, fout);
			}
		}

		private static void ProcessFile (FileInfo fi, FileInfo fo)
		{
			StringBuilder sb = new StringBuilder ();

			foreach (string line in File.ReadAllLines (fi.FullName)) {
				string newline = line;
				if (line.Contains("#include"))
				{
				}
				sb.AppendLine (newline);
			}
			File.WriteAllText (fo.FullName, sb.ToString ());
		}
	}
}
