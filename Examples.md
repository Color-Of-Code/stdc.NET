# Stdc Examples

## Stdc - Helps porting C code to .NET

## Rationale

Porting C code to .NET doesn't sound like being fun. And it is mostly for sure not funny...
The mind breaking rewriting of `printf` formatting code to String.Format formats can cost a bunch of time and is error prone.

The same difficulties arise with `scanf`, really tedious to port such code.

Signal handling is another topic where one can loose quite some hours.

Stdc is a pure .NET library enabling a quick port of existing C code by emulating most of the C syntax in a very similar way,
to not say in an identical manner.
The code can be then refactored step by step further by removing the C functions.
The Stdc libray enables a quick first shot so you have at least a running executable to work with.

Note that the Stdc library is written in pure .NET core. No call to native functions is made (no "cheating" with P/Invoke, to call the native C runtime methods is performed).

This can be important for portability between .NET on Linux and Windows for example.

A ported program should then run without recompilation under different platforms.

## Who cares about Stdc?

If you...

- have to quickly port a large portion of code and defer a clean .NET implementation to later on
- are not used to the .NET framework and want to mimic the C API in .NET
- wonder how to port some parts of the code but want a working result right now

## Examples

### Hello world

```c
    #include <stdio.h>

    void main(void) {
        printf("Hello World!\n");
    }
```

```csharp
namespace examples;    // a namespace to contain the code
using stdc;            // instead of #include ...

public class HelloWorld : C { // in C# methods must be in a class
    public static void main () {
        printf("Hello World!\n");
    }
}
```

The main difference is that the code must be embedded in a class and a namespace.
The functions turn consequently into public static methods (equivalent in .NET to C functions).

In further examples we will omit this necessary code parts to keep the focus on the real code changes.
Includes are replaced by deriving the class from a base `C` class which automatically makes the functions from `C` available.

Care was taken to put all the C functions into separate files using the `partial` implementation feature of C#. That way
in a real port, you can delete the portions of the C library that are not needed to save space or not pollute the namespace.

Therefore within a class derived from `C`, `printf` can be used as it would be the case in plain C.
This first example is simplistic but it is there just to get a feeling for the basic principles in porting C to .NET.

### Printing powers of 2 - printf()

```c
    #include <stdio.h>

    #define N 16

    void main(void) {
        int n;           /* The current exponent */
        int val = 1;     /* The current power of 2  */

        printf ("\t  n  \t    2^n\n");
        printf ("\t================\n");
        for (n=0; n<=N; n++) {
            printf ("\t%3d \t %6d\n", n, val);
            val = 2*val;
        }
    }
```

```csharp
using stdc;

public class PowerExample : C {

    private const int N = 16;

    public static void main () {
        int n;          // The current exponent
        int val = 1;    // The current power of 2

        printf ("\t  n  \t    2^n\n");
        printf ("\t================\n");
        for (n=0; n<=N; n++) {
            printf ("\t%3d \t %6d\n", n, val);
            val = 2 * val;
        }
    }
}
```

Note that absolutely no change was needed to be made to the formatting strings.

### Generating a file - FILE, fopen(), fclose(), putc()

```c
    #include <stdio.h>

    void main () {
        FILE * pFile;
        char c;

        pFile = fopen ("alphabet.txt", "wt");
        for (c = 'A' ; c <= 'Z' ; c++) {
            putc (c , pFile);
        }
        fclose (pFile);
    }
```

```csharp
using stdc;

public class FileExample : C {

    public static void main () {
        FILE pFile;
        char c;

        pFile = fopen ("alphabet.txt", "wt");
        for (c = 'A'; c <= 'Z'; c++) {
            putc (c, pFile);
        }
        fclose (pFile);
    }
}
```

The `fopen`, `fclose` can be used exactly like in C, only the pointer symbol (*) disappears.

### A small guessing game - rand(), scanf()

```c
    #include <stdio.h>
    #include <stdlib.h>
    #include <time.h>

    void main () {
        int iSecret, iGuess;

        srand ( time(NULL) );
        iSecret = rand() % 10 + 1;

        do {
            printf ("Guess the number (1 to 10): ");
            scanf ("%d",&iGuess);
            if (iSecret<iGuess)
                puts ("The secret number is lower");
            else if (iSecret>iGuess)
                puts ("The secret number is higher");
        } while (iSecret!=iGuess);

        puts ("Congratulations!");
    }
```

```csharp
using stdc;

public class GuessExample : C {

    public static void main () {
        int iSecret;
        object guess;
        int iGuess;

        srand (time (NULL));
        iSecret = rand () % 10 + 1;

        do {
            printf ("Guess the number (1 to 10): ");
            scanf ("%d", out guess);
            // can we get rid of this ugly casting here...
            iGuess = (int)guess;
            if (iSecret < iGuess)
                puts ("The secret number is lower");
            else if (iSecret > iGuess)
                puts ("The secret number is higher");
        } while (iSecret != iGuess);

        puts ("Congratulations!");
    }
}
```

The only ugly step needed here is the need for a cast, as `scanf` implementation is only able to handle object's as out parameters.
The API does not provide a solution for this dilemma right now.

### Quick Sort an array of ints, step by step refactoring - qsort()

```c
    #include <stdio.h>
    #include <stdlib.h>

    int values[] = { 40, 10, 100, 90, 20, 25 };

    int compare (const void * a, const void * b) {
        return ( *(int*)a - *(int*)b );
    }

    void main () {
        int n;
        qsort (values, 6, sizeof(int), compare);
        for (n=0; n<6; n++)
            printf ("%d ",values[n]);
    }
```

```csharp
using stdc;

public class SortExample : C {

    public static int[] values = new int[] { 40, 10, 100, 90, 20, 25 };

    public static int compare (int a, int b) {
        return a - b;
    }

    public static void main () {
        int n;
        qsort (values, 6, sizeof (int), compare);
        for (n = 0; n < 6; n++)
            printf ("%d ", values[n]);
    }
}
```

Second step, refactoring, getting rid of the C-like syntax and use .NET strengths.

We transformed the `for` loop into a `foreach` loop, making the use of the magic number '6' superfluous.

```csharp
using stdc;

public class SortExample : C {

    public static int[] values = new int[] { 40, 10, 100, 90, 20, 25 };

    public static int compare (int a, int b) {
        return a - b;
    }

    public static void main () {
        qsort (values, compare);
        foreach (int v in values)
            printf ("%d ", v);
    }
}
```

Third step: get rid of all C functions and replace them with their .NET equivalents. After that there
is no need anymore to use the base class `C` and the code is fully ported.

```csharp
using System;

public class SortExample {

    public static int[] values = new int[] { 40, 10, 100, 90, 20, 25 };

    public static int compare (int a, int b) {
        return a - b;
    }

    public static void main () {
        Array.Sort (values, compare);
        foreach (int v in values)
            Console.Write ("{0} ", v);
    }
}
```

Did you notice? From the first step on, the C# compare method didn't need any casts unlike the C version.
Thanks to the use of generics, the code readability is greaty improved.
This also shows the basic steps in refactoring the C code. Stdc just helps you to keep a testable running version
between successive steps of refactoring.

#### Remarks

In order to provide the advanced emulation functionality (like signals and atexit support, argc, argv emulation),
the library needs to control the code to be run. There is a trampoline from the .NET Main method to the ported main C function.

This should be used like this:

```csharp
namespace examples;
using stdc;

class Program : C {
    static void Main (string[] args)
    {
        // use one of these
        RunVMain (args, CProgram.main); // if the main is returning nothing (void)
        RunIMain (args, CProgram.main); // if the main is returning an int
    }
}
```

The signature of the main function is one of:

- `int main(int argc, string[] argv)`
- `int main()`
- `void main(int argc, string[] argv)`
- `void main()`

The .NET arguments do not contain the program name unlike in C where argv[0] contains the name of the executable.
To enable to reuse code from C that expects this behavior, you must use the `C.RunI/VMain()` function.
The `RunMain()` function also provides an environment where the `signal()` and `raise()` C functions can be used.

## Further examples

### Using atexit - atexit()

```c
    #include <stdio.h>
    #include <stdlib.h>

    void atexit_handler1 (void) {
        puts ("handler 1");
    }

    void atexit_handler2 (void) {
        puts ("handler 1");
    }

    void main () {
        atexit (atexit_handler1);
        atexit (atexit_handler2);
        puts ("atexit handlers should be " +
            "called in reverse order 2 and then 1!");
    }
```

```csharp
namespace examples;
using stdc;

public class Program: C {

    public static void atexit_handler1() {
        puts("handler 1");
    }
    public static void atexit_handler2() {
        puts("handler 2");
    }

    public static void main () {
        atexit(atexit_handler1);
        atexit(atexit_handler2);
        puts("atexit handlers should be " +
            "called in reverse order 2 and then 1!");
    }

    static void Main (string[] args) {
        RunVMain (args, main); // trampoline
    }
}
```

The full code is provided for this example, to demonstrate how to let the `RunMain()` method call the ported `main()` function.
`RunMain()` calls `main()` after initializing an environment where the signals can work properly.
The behavior expected from C regarding the order in which the handlers are called is implemented correctly: the handlers are called in reverse order or registration.
