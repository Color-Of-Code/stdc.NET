// generated by Csharpenizer (http://www.color-of-code.de)
#region Namespace wrapping
//css_reference stdc.dll;
using stdc;

namespace main {
  public partial class Program {
#endregion

//Compare two strings by soundex values// #include <stdio.h>// #include <error.h>// #define MAXLINE         1024int sndxcode[26];char *sndxgrp[] = { "aeiouhyw", "kcgjqsxz", "td",                    "bpfv", "l", "mn", "r" };void locsndx(char *, int *, char);int cmpcodes(int *, int *);int cmpsndx(char *, char *);int main(int argc, char *argv[]) { char *ptr = C.NULL; int i = 0; if(argc != 3)  error(1, 0, "string1 string2"); for(i = sizeof(sndxgrp) / sizeof(char *) - 1; i >= 0; i--)  for(ptr = sndxgrp[i]; *ptr; ptr++)   sndxcode[*ptr - 'a'] = i; C.printf("%d\n", cmpsndx(argv[1], argv[2])); return 0;}void locsndx(char *str, int *sndx, char lastchar) { if(0 == *str)  *sndx = -1, lastchar = 0; else if(*str == lastchar)  locsndx(str + 1, sndx, lastchar); else if(-1 == lastchar)  *sndx = sndxcode[*str - 'a'], locsndx(str + 1, sndx + 1, *str); else if(sndxcode[*str - 'a'] == 0)  locsndx(str + 1, sndx, *str); else  *sndx = sndxcode[*str - 'a'], locsndx(str + 1, sndx + 1, *str); return;}int cmpcodes(int *sndx1, int *sndx2) { int *ptr1 = sndx1; int *ptr2 = sndx2; for(; *ptr1 != -1 && *ptr2 != -1 && *ptr1 == *ptr2; ptr1++, ptr2++)  ; return *ptr1 == *ptr2;}int cmpsndx(char *str1, char *str2) { int[] sndx1 = new int[] {0};  int[] sndx2 = new int[] {0}; locsndx(str1, sndx1, -1); locsndx(str2, sndx2, -1); return cmpcodes(sndx1, sndx2);}

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
