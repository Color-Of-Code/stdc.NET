//#include <stdio.h>

//void main ()
//{
//  char str [80];
//  int i;

//  printf ("Enter your family name: ");
//  scanf ("%s",str);  
//  printf ("Enter your age: ");
//  scanf ("%d",&i);
//  printf ("Mr. %s, %d years old.\n",str,i);
//  printf ("Enter a hexadecimal number: ");
//  scanf ("%x",&i);
//  printf ("You have entered %#x (%d).\n",i,i);
//}

namespace examples
{

    using stdc;

    class Sscanf
    {

        public static void main()
        {
            object str; // we loose the type here due to limitations on 'out'
            object i;   // and 'params' doesn't seem of any help either...

            C.printf("Enter your family name: ");
            C.scanf("%s", out str);
            C.printf("Enter your age: ");
            C.scanf("%d", out i);
            C.printf("Mr. %s, %d years old.\n", str, i);
            C.printf("Enter a hexadecimal number: ");
            C.scanf("%x", out i);
            C.printf("You have entered %#x (%d).\n", i, i);
        }
    }
}
