// generated by Csharpenizer (https://github.com/Color-Of-Code/stdc.NET)
using stdc;

namespace main {
  public partial class Program {

/* printf example */
// #include <stdio.h>

int main()
{
   C.printf ("Characters: %c %c \n", 'a', 65);
   C.printf ("Decimals: %d %ld\n", 1977, 650000L);
   C.printf ("Preceding with blanks: %10d \n", 1977);
   C.printf ("Preceding with zeros: %010d \n", 1977);
   C.printf ("Some different radixes: %d %x %o %#x %#o \n", 100, 100, 100, 100, 100);
   C.printf ("floats: %4.2f %+.0e %E \n", 3.1416, 3.1416, 3.1416);
   C.printf ("Width trick: %*d \n", 5, 10);
   C.printf ("%s \n", "A string");
   return 0;
}


    static int Main (string[] args) {
      var p = new Program();
      return C.RunIMain (args, p.main);
    }
  }
}
