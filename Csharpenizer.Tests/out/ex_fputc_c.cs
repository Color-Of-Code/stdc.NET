// generated by Csharpenizer (https://color-of-code.de)
#region Namespace wrapping
//css_reference stdc.dll;
using stdc;

namespace main {
  public partial class Program {
#endregion

/* fputc example: alphabet writer */
// #include <stdio.h>

int main ()
{
  C.FILE pFile;
  char c;

  pFile = C.fopen ("ex_fputc.txt","w");
  if (pFile!=C.NULL)
  {
    for (c = 'A' ; c <= 'Z' ; c++)
    {
      C.fputc ( (int) c , pFile );
    }
    C.fclose (pFile);
  }
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
