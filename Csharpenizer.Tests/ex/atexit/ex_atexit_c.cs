// generated by Csharpenizer (https://color-of-code.de)
using stdc;

namespace main {
  public partial class Program {

/* atexit example */
// #include <stdio.h>
// #include <stdlib.h>

void fnExit1 ()
{
  C.puts ("Exit function 1.");
}

void fnExit2 ()
{
  C.puts ("Exit function 2.");
}

int main ()
{
  C.atexit (fnExit1);
  C.atexit (fnExit2);
  C.puts ("Main function.");
  return 0;
}


    static int Main (string[] args) {
      var p = new Program();
      return C.RunIMain (args, p.main);
    }
  }
}
