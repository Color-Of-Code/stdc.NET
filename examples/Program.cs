namespace examples;

public partial class Program : stdc.C
{
    static void Main (string[] args)
    {
        new Program().RunMain(args);
    }

    private void RunMain(string[] args)
    {
        // Work in progress
        //printf("------- Snake\n");
        //RunVMain (args, Snake.main);

        printf("------- Macros\n");
        RunVMain (args, Macros.main);

        printf("------- Sscanf\n");
        RunVMain (args, Sscanf.main);

        printf("------- Qsort\n");
        RunVMain (args, Qsort.main);

        printf("------- Random1\n");
        RunVMain (args, Random.main_srand);

        printf("------- Random2\n");
        RunVMain (args, Random.main_rand);

        printf("------- Atexit\n");
        RunVMain (args, Atexit.main);

        // printf("------- Signal\n");
        // RunVMain (args, Signal.main_term);

        printf("------- Signal\n");
        RunVMain (args, Signal.main_fpe);

        printf("------- HelloWorld\n");
        RunVMain (args, HelloWorld.main);

        // printf("------- Assert\n");
        // RunVMain (args, Assert.main);

        printf("------- Strcat1\n");
        RunVMain (args, Strcat.main);

        printf("------- Strcat2\n");
        RunVMain (args, Strcat.main2);

        printf("------- Strncpy1\n");
        RunVMain (args, Strncpy.main);

        printf("------- Strncpy2\n");
        RunVMain (args, Strncpy.main2);

        printf("------- Power2\n");
        RunIMain (args, Power2.main);

        printf("------- Alphabet\n");
        RunVMain (args, Alphabet.main);

        printf("------- Filecopy\n");
        RunIMain (args, FileCopy.main);
    }

    // int main()
    // {
    //     char[] str = new char[80];
    //     int i;

    //     printf("Enter your family name: ");
    //     scanf("%s", out str);
    //     printf("Enter your age: ");
    //     scanf("%d", out i);
    //     printf("Mr. %s , %d years old.\n", str, i);
    //     printf("Enter a hexadecimal number: ");
    //     scanf("%x", out i);
    //     printf("You have entered %#x (%d).\n", i, i);

    //     return 0;
    // }

    // #region Main trampoline
    // static int Main(string[] args)
    // {
    //     var p = new Program();
    //     return RunIMain(args, p.main);
    // }
    // #endregion
}
