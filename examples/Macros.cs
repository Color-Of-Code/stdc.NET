namespace examples;

using stdc;

// #include <stdio.h>

// void main() {
//     FILE *fp;
//     int c;
//
//     printf("Source: %s, L%i", __FILE__, __LINE__);
//     fp = fopen(__FILE__,"r");
//
//     do {
//          c = getc(fp);
//          putchar(c);
//     }
//     while(c != EOF);
//
//     fclose(fp);
// }

class Macros : C
{

    public static void main()
    {
        FILE fp;
        int c;

        printf("Source: %s, L%i\n", __FILE__(), __LINE__());
        fp = fopen(__FILE__(),"r");

        do {
            c = getc(fp);
            putchar(c);
        }
        while(c != EOF);

        fclose(fp);
    }
}
