// generated by Csharpenizer (https://github.com/Color-Of-Code/stdc.NET)
using stdc;

namespace main {
  public partial class Program {


// #include<stdio.h>
// #include<stdlib.h>
// #include<conio.h>

void main()
{
	int d,m,y,year,month,day,i,n;

	C.clrscr(); // non standard but supported
	C.printf("Enter how many times you want to run this program : ");
	C.scanf("%d",out n);
	for(i=1;i<=n;i++)
	{
		C.printf("Enter the date [day month year]: ");
		C.scanf("%d%d%d",out d,out m,out y);
		if( d>31 || m>12 || (y<1900 || y>=2000) )
		{
			C.printf("INVALID INPUT");
			C.getch();
			C.exit(0);
		}
		year = y-1900;
		year = year/4;
		year = year+y-1900;
		switch(m)
		{
		case 1:
		case 10:
			month = 1;
			break;
		case 2:
		case 3:
		case 11:
			month = 4;
			break;
		case 7:
		case 4:
			month = 0;
			break;
		case 5:
			month = 2;
			break;
		case 6:
			month = 5;
			break;
		case 8:
			month = 3;
			break;
		case 9:
		case 12:
			month = 6;
			break;
		}
		year = year+month;
		year = year+d;
		day  = year%7;
		switch(day)
		{
		case 0:
			C.printf("Day is SATURDAY");
			break;
		case 1:
			C.printf("Day is SUNDAY");
			break;
		case 2:
			C.printf("Day is MONDAY");
			break;
		case 3:
			C.printf("Day is TUESDAY");
			break;
		case 4:
			C.printf("Day is WEDNESDAY");
			break;
		case 5:
			C.printf("Day is THURSDAY");
			break;
		case 6:
			C.printf("Day is FRIDAY");
			break;
		}
		C.printf("\n---\n");
	}
	C.getch();
}



    static int Main (string[] args) {
      var p = new Program();
      C.RunVMain (args, p.main);
      return 0;
    }
  }
}
