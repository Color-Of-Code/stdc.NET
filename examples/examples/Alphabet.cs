
//#include <stdio.h>

//int main ()
//{
//  FILE * pFile;
//  char c;

//  pFile=fopen("alphabet.txt","wt");
//  for (c = 'A' ; c <= 'Z' ; c++) {
//    putc (c , pFile);
//    }
//  fclose (pFile);
//  return 0;
//}

namespace examples {

	//#include <stdio.h>
	using stdc;

	public class Alphabet {

		public static int main ()
		{
			FILE pFile;
			char c;

			pFile = C.fopen ("alphabet.txt", "wt");
			for (c = 'A'; c <= 'Z'; c++) {
				C.putc (c, pFile);
			}
			C.fclose (pFile);
			return 0;
		}

	}
}
