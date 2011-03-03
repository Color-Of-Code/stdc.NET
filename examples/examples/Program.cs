using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace examples {

	using stdc;

	class Program {
		static void Main (string[] args)
		{
			C.RunVMain (args, Qsort.main);
			C.RunVMain (args, Random.main_srand);
			C.RunVMain (args, Random.main_rand);
			C.RunVMain (args, Atexit.main);
			C.RunVMain (args, Signal.main_term);
			C.RunVMain (args, Signal.main_fpe);
			C.RunVMain (args, HelloWorld.main);
			C.RunVMain (args, Assert.main);
			C.RunVMain (args, Strcat.main);
			C.RunVMain (args, Strcat.main2);
			C.RunVMain (args, Strncpy.main);
			C.RunVMain (args, Strncpy.main2);
			C.RunIMain (args, Power2.main);
			C.RunIMain (args, Alphabet.main);
			C.RunIMain (args, FileCopy.main);
		}
	}
}
