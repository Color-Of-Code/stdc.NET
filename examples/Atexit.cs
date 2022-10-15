//#include <stdio.h>
//#include <stdlib.h>

//void atexit_handler1 (void) {
//    puts ("handler 1");
//}

//void atexit_handler2 (void) {
//    puts ("handler 1");
//}

//void main () {
//    atexit (atexit_handler1);
//    atexit (atexit_handler2);
//    puts ("atexit handlers should be called in reverse order 2 and then 1!");
//}

namespace examples;

public class Atexit : stdc.C
{

    public static void atexit_handler1()
    {
        puts("handler 1");
    }

    public static void atexit_handler2()
    {
        puts("handler 2");
    }

    public static void main()
    {
        atexit(atexit_handler1);
        atexit(atexit_handler2);
        puts("atexit handlers should be called in reverse order 2 and then 1!");
    }

}
