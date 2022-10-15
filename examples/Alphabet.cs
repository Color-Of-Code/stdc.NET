
//#include <stdio.h>
//
//void main ()
//{
//    FILE * pFile;
//    char c;
//
//    pFile=fopen("alphabet.txt","wt");
//    for (c = 'A' ; c <= 'Z' ; c++) {
//        putc (c , pFile);
//    }
//    fclose (pFile);
//}

namespace examples;

//#include <stdio.h>
using stdc;

public class Alphabet : C
{

    public static void main()
    {
        FILE pFile;
        char c;

        pFile = fopen("alphabet.txt", "wt");
        for (c = 'A'; c <= 'Z'; c++)
        {
            putc(c, pFile);
        }
        fclose(pFile);
    }

}
