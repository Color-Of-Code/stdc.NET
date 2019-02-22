using System;
using System.Threading;

namespace examples
{

    using stdc;

    public class Signal
    {

        public static void sigterm_handler(int p)
        {
            C.puts("Received SIGTERM");
        }

        public static void sigfpe_handler(int p)
        {
            C.puts("Received SIGFPE");
        }

        public static void main_term()
        {
            C.signal(C.SIGTERM, sigterm_handler);
            C.puts("Push Ctrl+C to break the process");
            while (true)
            {
                Thread.Sleep(100);
            }
        }

        public static void main_fpe()
        {
            C.signal(C.SIGFPE, sigfpe_handler);
            int j;
            for (int i = 0; i < 10; i++)
                j = i / i;
        }
    }
}
