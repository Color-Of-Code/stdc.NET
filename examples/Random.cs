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

namespace examples;

//#include <stdio.h>
//#include <stdlib.h>
//#include <time.h>
using stdc;

public class Random : C
{

    public static void main_rand()
    {
        int iSecret;
        object guess;
        int iGuess;

        srand(time(NULL));
        iSecret = rand() % 10 + 1;

        do
        {
            printf("Guess the number (1 to 10): ");
            scanf("%d", out guess);
            iGuess = (int)guess;    // can we get rid of this ugly casting here...
            if (iSecret < iGuess)
                puts("The secret number is lower");
            else if (iSecret > iGuess)
                puts("The secret number is higher");
        } while (iSecret != iGuess);

        puts("Congratulations!");
    }

    public static void main_srand()
    {
        printf("First number: %d\n", rand() % 100);
        srand(time(NULL));
        printf("Random number: %d\n", rand() % 100);
        srand(1);
        printf("Again the first number: %d\n", rand() % 100);
    }

}
