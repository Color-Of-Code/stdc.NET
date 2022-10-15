namespace stdc;

public partial class C
{

    /// <summary>
    /// Last error number
    ///
    /// This macro expands to a modifiable lvalue of type int, therefore it
    /// can be both read and modified by a program.
    ///
    /// errno is set to zero at program startup, and certain functions of the
    /// standard C library modify its value to some value different from zero
    /// to signal some types of error. You can also modify its value or reset
    /// to zero at your convenience.
    ///
    /// The same header that declares errno (&lt;cerrno&gt;) also declares at least
    /// the following two macro constants with values different from zero:
    ///
    /// macro	meaning when errno is set to this
    /// EDOM	Domain error: Some mathematical functions are only defined for
    ///         certain real values, which is called its domain, for example
    ///         the square root function is only defined for non-negative
    ///         numbers, therefore the sqrt function sets errno to EDOM if
    ///         called with a negative argument.
    /// ERANGE	Range error: The range of values that can be represented with
    ///         a variable is limited. For example, mathematical functions such
    ///         as pow can easily outbound the range representable by a floating
    ///         point variable, or functions such as strtod can encounter
    ///         sequences of digits longer than the range representable by an
    ///         int value. In these cases, errno is set to ERANGE.
    ///
    /// In C++, errno is always declared as a macro, but in C compilers it may
    /// also be implemented as an int object with external linkage.
    /// </summary>
    public static int errno = 0;

    /// <summary>
    /// All errno values, defined as predefined constants in ERRNO.H, are
    /// UNIX-compatible and are listed below. Only ERANGE, EILSEQ and EDOM
    /// are specified in the ANSI standard.
    /// </summary>
    public const int EPERM = 1; // Operation not permitted
    public const int ENOENT = 2;    // No such file or directory
    public const int ESRCH = 3; // No such process
    public const int EINTR = 4; // Interrupted function
    public const int EIO = 5;   // I/O error
    public const int ENXIO = 6; // No such device or address
    public const int E2BIG = 7; // Argument list too long
    public const int ENOEXEC = 8;   // Exec format error
    public const int EBADF = 9; // Bad file number
    public const int ECHILD = 10;   // No spawned processes
    public const int EAGAIN = 11;   // No more processes or not enough memory or maximum nesting level reached
    public const int ENOMEM = 12;   // Not enough memory
    public const int EACCES = 13;   // Permission denied
    public const int EFAULT = 14;   // Bad address

    public const int EBUSY = 16;    // Device or resource busy
    public const int EEXIST = 17;   // File exists
    public const int EXDEV = 18;    // Cross-device link
    public const int ENODEV = 19;   // No such device
    public const int ENOTDIR = 20;  // Not a directory
    public const int EISDIR = 21;   // Is a directory
    public const int EINVAL = 22;   // Invalid argument
    public const int ENFILE = 23;   // Too many files open in system
    public const int EMFILE = 24;   // Too many open files
    public const int ENOTTY = 25;   // Inappropriate I/O control operation

    public const int EFBIG = 27;    // File too large
    public const int ENOSPC = 28;   // No space left on device
    public const int ESPIPE = 29;   // Invalid seek
    public const int EROFS = 30;    // Read-only file system
    public const int EMLINK = 31;   // Too many links
    public const int EPIPE = 32;    // Broken pipe
    public const int EDOM = 33; // Math argument			(ANSI)
    public const int ERANGE = 34;   // Result too large			(ANSI)

    public const int EDEADLK = 36;  // Resource deadlock would occur
    public const int EDEADLOCK = 36;    // Same as EDEADLK for compatibility with older Microsoft C versions

    public const int ENAMETOOLONG = 38; // Filename too long
    public const int ENOLCK = 39;   // No locks available
    public const int ENOSYS = 40;   // Function not supported
    public const int ENOTEMPTY = 41;    // Directory not empty
    public const int EILSEQ = 42;   // Illegal byte sequence	(ANSI)

    public const int STRUNCATE = 80;    // String was truncated
}
