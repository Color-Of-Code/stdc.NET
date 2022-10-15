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

namespace examples;

using System.Text;

public class Strncpy : stdc.C
{
    public static void main()
    {
        string str1 = "To be or not to be";
        char[] str2 = new char[6];
        strncpy(str2, str1, 5);
        str2[5] = '\0';
        puts(str2);
    }

    public static void main2()
    {
        string str1 = "To be or not to be";
        StringBuilder str2 = new StringBuilder(6);
        strncpy(str2, str1, 5);
        puts(str2);
    }
}
