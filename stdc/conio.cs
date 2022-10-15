namespace stdc;

using System;

public partial class C
{
    // This function is not defined as part of the ANSI C/C++ standard.
    // It is generally used by Borland's family of compilers.
    [Obsolete("non standard conio.h support")]
    public static void clrscr()
    {
        Console.Clear();
    }

    // It returns a non-zero integer if a key is in the keyboard buffer. It will not wait for a key to be pressed.
    // NOTE: the official signature was changed from int to bool, to make the if's look less cluttered on usage.
    public static bool kbhit() => Console.KeyAvailable;

    // Gets a key code from the console without echo.
    public static int getch()
    {
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow: return 72;
            case ConsoleKey.LeftArrow: return 75;
            case ConsoleKey.RightArrow: return 77;
            case ConsoleKey.DownArrow: return 80;
            case ConsoleKey.Enter: return 13;
            case ConsoleKey.Escape: return 27;
            case ConsoleKey.P: return 112;
        }
        return (int)key;
    }

    // gotoxy
    // gotoxy function places cursor at a desired location on screen i.e., we can change cursor position using gotoxy function.
    public static void gotoxy(int x, int y)
    {
        Console.CursorLeft = x;
        Console.CursorTop = y;
    }
}
