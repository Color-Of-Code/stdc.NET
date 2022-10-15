//#include <stdio.h>
//#include <assert.h>

//void print_number(int* myInt) {
//  assert (myInt!=NULL);
//  printf ("%d\n",*myInt);
//}

//void main ()
//{
//  int a=10;
//  int * b = NULL;
//  int * c = NULL;
//  b=&a;
//  print_number (b);
//  print_number (c);
//}

namespace examples;

//#include <stdio.h>
//#include <assert.h>
using stdc;

public class Assert : C
{

    public static void print_number(object myInt)
    {
        assert(myInt != NULL);
        printf("%d\n", myInt);
    }

    public static void main()
    {
        int a = 10;
        var b = NULL;
        var c = NULL;
        b = a;
        print_number(b);
        print_number(c);
    }
}
