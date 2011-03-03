
using System;
using System.Text;

namespace examples {

	using stdc;

	public class Atexit {

		public static void atexit_handler1()
		{
			C.puts("handler 1");
		}

		public static void atexit_handler2()
		{
			C.puts("handler 2");
		}

		public static void main ()
		{
			C.atexit(atexit_handler1);
			C.atexit(atexit_handler2);
			C.puts("atexit handlers should be called in reverse order 2 and then 1!");
		}
	
	}
}
