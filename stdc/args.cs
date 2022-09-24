namespace stdc;

using System;

public static partial class C
{

    public delegate int imain1(int argc, string[] argv);
    public delegate int imain2();
    public delegate void vmain1(int argc, string[] argv);
    public delegate void vmain2();

    public static int RunIMain(string[] args, imain1 main)
    {
        int argc = C.ToArgc(args);
        string[] argv = C.ToArgv(args);
        int ret = 0;
        try
        {
            ret = main(argc, argv);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            ret = -1;
        }
        RunAtExitHandlers();
        return ret;
    }

    public static int RunIMain(string[] args, imain2 main)
    {
        int ret = 0;
        try
        {
            ret = main();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            ret = -1;
        }
        RunAtExitHandlers();
        return ret;
    }

    public static int RunVMain(string[] args, vmain1 main)
    {
        int ret = 0;
        try
        {
            int argc = C.ToArgc(args);
            string[] argv = C.ToArgv(args);
            main(argc, argv);
            return ret;
        }
        catch (Exception ex)
        {
            HandleException(ex);
            ret = -1;
        }
        RunAtExitHandlers();
        return ret;
    }

    public static int RunVMain(string[] args, vmain2 main)
    {
        int ret = 0;
        try
        {
            main();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            ret = -1;
        }
        RunAtExitHandlers();
        return ret;
    }

    private static void RunAtExitHandlers()
    {
        foreach (var handler in _atexitHandlers)
            handler();
    }

    /// <summary>
    /// Build the standard C argv array from the .NET argument list.
    /// The .NET variant does not contain the program name. so we emulate
    /// it by injecting the application domain friendly name.
    /// </summary>
    /// <param name="args">The .NET argument list</param>
    /// <returns></returns>
    private static string[] ToArgv(string[] args)
    {
        string[] argv = new string[args.Length + 1];
        Array.Copy(args, 0, argv, 1, args.Length);
        argv[0] = System.AppDomain.CurrentDomain.FriendlyName;
        return argv;
    }

    /// <summary>
    /// The number of arguments in the array (1 more than in the original
    /// .NET array)
    /// </summary>
    /// <param name="args">The .NET argument list</param>
    /// <returns></returns>
    private static int ToArgc(string[] args)
    {
        return args.Length + 1;
    }

    private static void HandleException(Exception ex)
    {
        if (ex is DivideByZeroException)
        {
            _sigfpeHandler(SIGFPE);
            return;
        }
        if (ex is OverflowException)
        {
            _sigfpeHandler(SIGFPE);
            return;
        }
        if (ex is IndexOutOfRangeException)
        {
            _sigsegvHandler(SIGSEGV);
            return;
        }
        if (ex is AccessViolationException)
        {
            _sigsegvHandler(SIGSEGV);
            return;
        }
        if (ex is SystemException)
        {
            _sigillHandler(SIGILL);
            return;
        }

        // default in case of exception
        _sigabrtHandler(SIGABRT);
    }

}
