//#include <stdio.h>
//#include <string.h>

//void main ()
//{
//  char str1[]= "To be or not to be";
//  char str2[6];
//  strncpy (str2,str1,5);
//  str2[5]='\0';
//  puts (str2);
//}

namespace examples {

	//#include <stdio.h>
	//#include <string.h>
	using stdc;
	using System.Text;

	public class Strncpy {
		public static void main ()
		{
			string str1 = "To be or not to be";
			char[] str2 = new char[6];
			C.strncpy (str2, str1, 6);
			str2[5] = '\0';
			C.puts (str2);
		}

		public static void main2 ()
		{
			string str1 = "To be or not to be";
			StringBuilder str2 = new StringBuilder (6);
			C.strncpy (str2, str1, 6);
			C.puts (str2);
		}
	}
}
