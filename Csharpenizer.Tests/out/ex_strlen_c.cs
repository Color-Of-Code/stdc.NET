// generated by Csharpenizer (https://color-of-code.de)
#region Namespace wrapping
//css_reference stdc.dll;
using stdc;

namespace main {
  public partial class Program {
#endregion

/* strlen example */
// #include <stdio.h>
// #include <string.h>

int main ()
{
  char[] szInput = new char[256];
  C.printf ("Enter a sentence: ");
  C.gets (szInput);
  C.printf ("The sentence entered is %u characters long.\n",C.strlen(szInput));
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
