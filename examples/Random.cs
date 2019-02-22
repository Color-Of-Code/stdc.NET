// rand example
//#include <stdio.h>
//#include <stdlib.h>
//#include <time.h>

//void main () {
//    int iSecret, iGuess;

//    srand ( time(NULL) );
//    iSecret = rand() % 10 + 1;

//    do {
//        printf ("Guess the number (1 to 10): ");
//        scanf ("%d",&iGuess);
//        if (iSecret<iGuess) puts ("The secret number is lower");
//        else if (iSecret>iGuess) puts ("The secret number is higher");
//    } while (iSecret!=iGuess);

//    puts ("Congratulations!");
//}

// srand example
//#include <stdio.h>
//#include <stdlib.h>
//#include <time.h>

//void main ()
//{
//  printf ("First number: %d\n", rand() % 100);
//  srand ( time(NULL) );
//  printf ("Random number: %d\n", rand() % 100);
//  srand ( 1 );
//  printf ("Again the first number: %d\n", rand() %100);
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace examples
{

    //#include <stdio.h>
    //#include <stdlib.h>
    //#include <time.h>
    using stdc;

    public class Random
    {

        public static void main_rand()
        {
            int iSecret;
            object guess;
            int iGuess;

            C.srand(C.time(C.NULL));
            iSecret = C.rand() % 10 + 1;

            do
            {
                C.printf("Guess the number (1 to 10): ");
                C.scanf("%d", out guess);
                iGuess = (int)guess;    // can we get rid of this ugly casting here...
                if (iSecret < iGuess)
                    C.puts("The secret number is lower");
                else if (iSecret > iGuess)
                    C.puts("The secret number is higher");
            } while (iSecret != iGuess);

            C.puts("Congratulations!");
        }

        public static void main_srand()
        {
            C.printf("First number: %d\n", C.rand() % 100);
            C.srand(C.time(C.NULL));
            C.printf("Random number: %d\n", C.rand() % 100);
            C.srand(1);
            C.printf("Again the first number: %d\n", C.rand() % 100);
        }

    }
}
