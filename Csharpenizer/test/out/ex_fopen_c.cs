// generated by Csharpenizer (http://www.color-of-code.de)
#region Namespace wrapping
//css_reference stdc.dll;
using stdc;

namespace main {
  public partial class Program {
#endregion

/*[  fopen example  ]*/#include <stdio.h>int main (){  FILE * pFile;  pFile = fopen ("myfile.txt","w");  if (pFile!=NULL)  {    fputs ("fopen example",pFile);    fclose (pFile);  }  return 0;}
#region Namespace wrapping
  }
}
#endregion
