using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace examples {

	using stdc;

	class Program {
		static void Main (string[] args)
		{
			HelloWorld.main ();
			//Assert.main ();
			Strcat.main ();
			Strcat.main2 ();
			Strncpy.main ();
			Strncpy.main2 ();
			Power2.main ();
			Alphabet.main ();
			FileCopy.main (C.ToArgc (args), C.ToArgv (args));
		}
	}
}
