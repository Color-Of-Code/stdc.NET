//#include <stdio.h>
//#include <stdlib.h>
//
//int main(int argc, char * argv[]) {
//  FILE *fin, *fout;
//  char c;
//
//  if (argc!=3) {
//    printf("Usage: %s filein fileout\n", argv[0]);
//    exit(0);
//  }
//  if ((fin=fopen(argv[1],"r"))==NULL) {
//    perror("fopen filein");
//    exit(0);
//  }
//  if ((fout=fopen(argv[2],"w"))==NULL) {
//    perror("fopen fileout");
//    exit(0);
//  }
//
//  while ((c=getc(fin))!=EOF)
//    putc(c,fout);
//
//  fclose(fin);
//  fclose(fout);
//  return 0;
//}


namespace examples;

//#include <stdio.h>
//#include <stdlib.h>
using stdc;

public class FileCopy : C
{

    //int main(int argc, char * argv[])
    public static int main(int argc, string[] argv)
    {
        FILE fin, fout;
        int c;  // char would need explicit conversions with the current API
                // and the comparison with EOF would always fail, resulting in an
                // endless loop!

        if (argc != 3)
        {
            printf("Usage: %s filein fileout\n", argv[0]);
            exit(0);
        }
        if ((fin = fopen(argv[1], "rb")) == NULL)
        {
            perror("fopen filein");
            exit(0);
        }
        if ((fout = fopen(argv[2], "wb")) == NULL)
        {
            perror("fopen fileout");
            exit(0);
        }

        while ((c = getc(fin)) != EOF)
            putc(c, fout);

        fclose(fin);
        fclose(fout);
        return 0;
    }
}
