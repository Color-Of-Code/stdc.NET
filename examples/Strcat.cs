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

namespace examples;

using System.Text;

public class Strcat : stdc.C
{
    public static void main()
    {
        char[] str = new char[80];
        strcpy(str, "these ");
        strcat(str, "strings ");
        strcat(str, "are ");
        strcat(str, "concatenated.");
        puts(str);
    }

    public static void main2()
    {
        StringBuilder str = new StringBuilder(80);
        strcpy(str, "these ");
        strcat(str, "strings ");
        strcat(str, "are ");
        strcat(str, "concatenated.");
        puts(str);
    }
}
