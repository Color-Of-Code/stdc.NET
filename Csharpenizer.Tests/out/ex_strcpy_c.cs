// generated by Csharpenizer (http://www.color-of-code.de)
#region Namespace wrapping
//css_reference stdc.dll;
using stdc;

namespace main {
  public partial class Program {
#endregion

/* strcpy example */
// #include <stdio.h>
// #include <string.h>

int main ()
{
  string str1="Sample string";
  char[] str2 = new char[40];
  char[] str3 = new char[40];
  C.strcpy (str2,str1);
  C.strcpy (str3,"copy successful");
  C.printf ("str1: %s\nstr2: %s\nstr3: %s\n",str1,str2,str3);
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