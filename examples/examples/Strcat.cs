//#include <stdio.h>
//#include <string.h>

//void main ()
//{
//  char str[80];
//  strcpy (str,"these ");
//  strcat (str,"strings ");
//  strcat (str,"are ");
//  strcat (str,"concatenated.");
//  puts (str);
//}

namespace examples {
	//#include <stdio.h>
	//#include <string.h>
	using stdc;
	using System.Text;

	public class Strcat {

		public static void main ()
		{
			char[] str = new char[80];
			C.strcpy (str, "these ");
			C.strcat (str, "strings ");
			C.strcat (str, "are ");
			C.strcat (str, "concatenated.");
			C.puts (str);
		}

		public static void main2 ()
		{
			StringBuilder str = new StringBuilder(80);
			C.strcpy (str, "these ");
			C.strcat (str, "strings ");
			C.strcat (str, "are ");
			C.strcat (str, "concatenated.");
			C.puts (str);
		}
	}
}
