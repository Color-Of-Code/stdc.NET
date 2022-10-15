//#include <stdio.h>
//#define N 16

//int main(void) {
//  int n;           /* The current exponent */
//  int val = 1;     /* The current power of 2  */

//  printf("\t  n  \t    2^n\n");
//  printf("\t================\n");
//  for (n=0; n<=N; n++) {
//    printf("\t%3d \t %6d\n", n, val);
//    val = 2*val;
//  }
//  return 0;
//}

/* It prints out :

	  n  	    2^n
	================
	  0 	      1
	  1 	      2
	  2 	      4
	  3 	      8
	  4 	     16
	  5 	     32
	  6 	     64
	  7 	    128
	  8 	    256
	  9 	    512
	 10 	   1024
	 11 	   2048
	 12 	   4096
	 13 	   8192
	 14 	  16384
	 15 	  32768
	 16 	  65536

*/

namespace examples;

//#include <stdio.h>
using stdc;

public class Power2 : C
{

    private const int N = 16;

    public static int main()
    {
        int n;          // The current exponent
        int val = 1;    // The current power of 2

        printf("\t  n  \t    2^n\n");
        printf("\t================\n");
        for (n = 0; n <= N; n++)
        {
            printf("\t%3d \t %6d\n", n, val);
            val = 2 * val;
        }
        return 0;
    }
}
