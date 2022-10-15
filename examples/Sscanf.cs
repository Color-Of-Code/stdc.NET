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

namespace examples;

class Sscanf : stdc.C
{

    public static void main()
    {
        object str; // we loose the type here due to limitations on 'out'
        object i;   // and 'params' doesn't seem of any help either...

        printf("Enter your family name: ");
        scanf("%s", out str);
        printf("Enter your age: ");
        scanf("%d", out i);
        printf("Mr. %s, %d years old.\n", str, i);
        printf("Enter a hexadecimal number: ");
        scanf("%x", out i);
        printf("You have entered %#x (%d).\n", i, i);
    }
}
