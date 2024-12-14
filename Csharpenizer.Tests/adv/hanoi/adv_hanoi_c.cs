// generated by Csharpenizer (https://github.com/Color-Of-Code/stdc.NET)
using stdc;

namespace main {
  public partial class Program {

// #include <stdio.h>

int i=1;

void solution(int n,char from,char to,char temp)
{
	if(n>0)
	{
		solution(n-1,from,temp,to);
		C.printf("step %d -> Move the disk %d form %c to %c.\n",i++,n,from,to);
		solution(n-1,temp,to,from);
	}
}

int main()
{
	int n;
	C.printf("Tower of hanoi problem.\n");
	C.printf("Enter the no. disks:");
	C.scanf("%d",out n);
	C.printf("The solution.\n");
	solution(n,'L','R','C');
	return 0;
}


    static int Main (string[] args) {
      var p = new Program();
      return C.RunIMain (args, p.main);
    }
  }
}
