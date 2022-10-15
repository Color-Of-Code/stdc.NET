namespace examples;

using System.Threading;

public class Signal : stdc.C
{
    public static void sigterm_handler(int p)
    {
        puts("Received SIGTERM");
    }

    public static void sigfpe_handler(int p)
    {
        puts("Received SIGFPE");
    }

    public static void main_term()
    {
        signal(SIGTERM, sigterm_handler);
        puts("Push Ctrl+C to break the process");
        while (true)
        {
            Thread.Sleep(100);
        }
    }

    public static void main_fpe()
    {
        signal(SIGFPE, sigfpe_handler);
        int j;
        for (int i = 0; i < 10; i++)
            j = i / i;
    }
}
