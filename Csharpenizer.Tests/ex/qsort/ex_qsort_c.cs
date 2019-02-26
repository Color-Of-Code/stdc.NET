// generated by Csharpenizer (https://color-of-code.de)
#region Namespace wrapping
//css_reference stdc.dll;
using stdc;

namespace main {
  public partial class Program {
#endregion

/* qsort example */
// #include <stdio.h>
// #include <stdlib.h>

int[] values = new int[] { 40, 10, 100, 90, 20, 25 };

int compare (int a, int b)
{
  return ( a - b );
}

int main ()
{
  int n;
  C.qsort (values, 6, sizeof(int), compare);
  for (n=0; n<6; n++)
     C.printf ("%d ",values[n]);
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