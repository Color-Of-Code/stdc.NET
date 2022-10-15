//#include <stdio.h>
//#include <stdlib.h>

//int values[] = { 40, 10, 100, 90, 20, 25 };

//int compare (const void * a, const void * b)
//{
//    return ( *(int*)a - *(int*)b );
//}

//void main ()
//{
//    int n;
//    qsort (values, 6, sizeof(int), compare);
//    for (n=0; n<6; n++)
//        printf ("%d ",values[n]);
//}
namespace examples;

//#include <stdio.h>
//#include <stdlib.h>
using stdc;

public class Qsort : C
{

    public static int[] values = new int[] { 40, 10, 100, 90, 20, 25 };

    public static int compare(int a, int b)
    {
        return a - b;
    }

    public static void main_1()
    {
        int n;
        qsort(values, 6, sizeof(int), compare);
        for (n = 0; n < 6; n++)
            printf("%d ", values[n]);
    }

    public static void main()
    {
        qsort(values, compare);
        foreach (int v in values)
            printf("%d ", v);
    }
}
