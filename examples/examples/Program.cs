using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace examples {
	using stdc;

	namespace main {
		public partial class Program {
			/* C.strcpy example */
			// #include <stdio.h>
			// #include <string.h>

			int main ()
			{
				string str1 = "Sample string";
				char[] str2 = new char[40];
				char[] str3 = new char[40];
				C.strcpy (str2, str1);
				C.strcpy (str3, "copy successful");
				C.printf ("str1: %s\nstr2: %s\nstr3: %s\n", str1, str2, str3);
				return 0;
			}
			static int Main (string[] args)
			{
				Program p = new Program ();
				return C.RunIMain (args, p.main);
			}
		}
	}

	//class Program {
	//    static void Main (string[] args)
	//    {
	//        C.RunVMain (args, Sscanf.main);
	//        C.RunVMain (args, Qsort.main);
	//        C.RunVMain (args, Random.main_srand);
	//        C.RunVMain (args, Random.main_rand);
	//        C.RunVMain (args, Atexit.main);
	//        //C.RunVMain (args, Signal.main_term);
	//        C.RunVMain (args, Signal.main_fpe);
	//        C.RunVMain (args, HelloWorld.main);
	//        C.RunVMain (args, Assert.main);
	//        C.RunVMain (args, Strcat.main);
	//        C.RunVMain (args, Strcat.main2);
	//        C.RunVMain (args, Strncpy.main);
	//        C.RunVMain (args, Strncpy.main2);
	//        C.RunIMain (args, Power2.main);
	//        C.RunVMain (args, Alphabet.main);
	//        C.RunIMain (args, FileCopy.main);
	//    }
	//}
}
