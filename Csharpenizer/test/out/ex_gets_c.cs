// generated by Csharpenizer (http://www.color-of-code.de)
#region Namespace wrapping
//css_reference stdc.dll;
using stdc;

namespace main {
  public partial class Program {
#endregion

/* gets example */
// #include <stdio.h>

int main()
{
  char[] string = new char[256];
  C.printf ("Insert your full address: ");
  C.gets (string);
  C.printf ("Your address is: %s\n",string);
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
