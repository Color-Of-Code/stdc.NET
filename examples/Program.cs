using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace examples
{
    using stdc;

    namespace main
    {
        public partial class Program
        {

            int main()
            {
                char[] str = new char[80];
                int i;

                C.printf("Enter your family name: ");
                C.scanf("%s", out str);
                C.printf("Enter your age: ");
                C.scanf("%d", out i);
                C.printf("Mr. %s , %d years old.\n", str, i);
                C.printf("Enter a hexadecimal number: ");
                C.scanf("%x", out i);
                C.printf("You have entered %#x (%d).\n", i, i);

                return 0;
            }

            #region Main trampoline
            static int Main(string[] args)
            {
                var p = newProgram();
                return C.RunIMain(args, p.main);
            }
            #endregion
        }
    }

    //class Program {
    //    static void Main (string[] args)
    //    {
    //        C.RunVMain (args, Sscanf.main);
    //        C.RunVMain (args, Qsort.main);
    //        C.RunVMain (args, Random.main_srand);
    //        C.RunVMain (args, Random.main_rand);
    //        C.RunVMain (args, Atexit.main);
    //        //C.RunVMain (args, Signal.main_term);
    //        C.RunVMain (args, Signal.main_fpe);
    //        C.RunVMain (args, HelloWorld.main);
    //        C.RunVMain (args, Assert.main);
    //        C.RunVMain (args, Strcat.main);
    //        C.RunVMain (args, Strcat.main2);
    //        C.RunVMain (args, Strncpy.main);
    //        C.RunVMain (args, Strncpy.main2);
    //        C.RunIMain (args, Power2.main);
    //        C.RunVMain (args, Alphabet.main);
    //        C.RunIMain (args, FileCopy.main);
    //    }
    //}
}
