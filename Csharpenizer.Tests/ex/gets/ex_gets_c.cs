// generated by Csharpenizer (https://github.com/Color-Of-Code/stdc.NET)
using stdc;

namespace main {
  public partial class Program {

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


    static int Main (string[] args) {
      var p = new Program();
      return C.RunIMain (args, p.main);
    }
  }
}
