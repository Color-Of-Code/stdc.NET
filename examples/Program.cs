namespace examples;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using stdc;

public partial class Program
{
    static void Main (string[] args)
    {
        C.printf("------- Sscanf\n");
        C.RunVMain (args, Sscanf.main);
        C.printf("------- Qsort\n");
        C.RunVMain (args, Qsort.main);
        C.printf("------- Random1\n");
        C.RunVMain (args, Random.main_srand);
        C.printf("------- Random2\n");
        C.RunVMain (args, Random.main_rand);
        C.printf("------- Atexit\n");
        C.RunVMain (args, Atexit.main);
        //C.printf("------- Signal\n");
        //C.RunVMain (args, Signal.main_term);
        C.printf("------- Signal\n");
        C.RunVMain (args, Signal.main_fpe);
        C.printf("------- HelloWorld\n");
        C.RunVMain (args, HelloWorld.main);
        // C.printf("------- Assert\n");
        // C.RunVMain (args, Assert.main);
        C.printf("------- Strcat1\n");
        C.RunVMain (args, Strcat.main);
        C.printf("------- Strcat2\n");
        C.RunVMain (args, Strcat.main2);
        C.printf("------- Strncpy1\n");
        C.RunVMain (args, Strncpy.main);
        C.printf("------- Strncpy2\n");
        C.RunVMain (args, Strncpy.main2);
        C.printf("------- Power2\n");
        C.RunIMain (args, Power2.main);
        C.printf("------- Alphabet\n");
        C.RunVMain (args, Alphabet.main);
        C.printf("------- Filecopy\n");
        C.RunIMain (args, FileCopy.main);
    }

    // int main()
    // {
    //     char[] str = new char[80];
    //     int i;

    //     C.printf("Enter your family name: ");
    //     C.scanf("%s", out str);
    //     C.printf("Enter your age: ");
    //     C.scanf("%d", out i);
    //     C.printf("Mr. %s , %d years old.\n", str, i);
    //     C.printf("Enter a hexadecimal number: ");
    //     C.scanf("%x", out i);
    //     C.printf("You have entered %#x (%d).\n", i, i);

    //     return 0;
    // }

    // #region Main trampoline
    // static int Main(string[] args)
    // {
    //     var p = new Program();
    //     return C.RunIMain(args, p.main);
    // }
    // #endregion
}
