using System;
using System.Collections.Generic;
using System.Text;

namespace stdc {
	public static partial class C {

		[Obsolete ("non standard conio.h support")]
		public static void clrscr ()
		{
			Console.Clear ();
		}

		/// <summary>
		/// Gets a character from the console without echo.
		/// </summary>
		/// <returns></returns>
		public static Char getch ()
		{
			return (Char)Console.In.Read();
		}
	}
}
