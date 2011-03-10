
#include<stdio.h>
#include<stdlib.h>
#include<conio.h>

void main()
{
	clrscr(); // non standard but supported
	int d,m,y,year,month,day,i,n;
	printf("Enter how many times you want to run this program : ");
	scanf("%d",&n);
	for(i=1;i<=n;i++)
	{
		printf("Enter the date [day month year]: ");
		scanf("%d%d%d",&d,&m,&y);
		if( d>31 || m>12 || (y<1900 || y>=2000) )
		{
			printf("INVALID INPUT");
			getch();
			exit(0);
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
			printf("Day is SATURDAY");
			break;
		case 1:
			printf("Day is SUNDAY");
			break;
		case 2:
			printf("Day is MONDAY");
			break;
		case 3:
			printf("Day is TUESDAY");
			break;
		case 4:
			printf("Day is WEDNESDAY");
			break;
		case 5:
			printf("Day is THURSDAY");
			break;
		case 6:
			printf("Day is FRIDAY");
			break;
		}
		printf("\n---\n");
	}
	getch();
}
