// generated by Csharpenizer (http://www.color-of-code.de)
#region Namespace wrapping
//css_reference stdc.dll;
using stdc;

namespace main {
  public partial class Program {
#endregion

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


    #region Main trampoline
    static int Main (string[] args) {
      Program p = new Program();
      return C.RunIMain (args, p.main);
    }
    #endregion
#region Namespace wrapping
  }
}
#endregion