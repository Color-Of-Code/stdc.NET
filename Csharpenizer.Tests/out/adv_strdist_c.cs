// generated by Csharpenizer (http://www.color-of-code.de)
#region Namespace wrapping
//css_reference stdc.dll;
using stdc;

namespace main {
  public partial class Program {
#endregion

//Another; compute string distance

// #include <stdio.h>
// #include <string.h>
// #include <error.h>

// #define MIN(x, y)        ((x < y) ? x : y)

int strdist(char *sone, char *stwo);

int main(int argc, char *argv[]) {

 if(argc != 3)
  error(1, 0, "str1 str2");

 C.printf("distance : %d\n", strdist(argv[1], argv[2]));

 return 0;
}

int strdist(char *sone, char *stwo) {
 int d1 = 0;
 int d2 = 0;
 int d3 = 0;

 if(*sone == 0)
  return C.strlen(stwo);
 if(*stwo == 0)
  return C.strlen(sone);
 if(*sone == *stwo)
  d1 = strdist((sone + 1), (stwo + 1));
 else
  d1 = 1 + strdist((sone + 1), (stwo + 1));

 d2 = 1 + strdist(sone, (stwo + 1));
 d3 = 1 + strdist((sone + 1), stwo);

 return MIN(d1, MIN(d2, d3));
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