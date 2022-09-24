// must be defined so that the asserts are present in the release build
// to be used from a debug compilation
#define DEBUG

namespace stdc;

using System;
using System.Diagnostics;

public static partial class C
{
    /// <summary>
    ///		void assert (int expression);
    ///
    /// Evaluate assertion
    /// If the argument expression of this macro with functional form
    /// compares equal to zero (i.e., the expression is false), a message
    /// is written to the standard error device and abort is called,
    /// terminating the program execution.
    ///
    /// The specifics of the message shown depend on the specific
    /// implementation in the compiler, but it shall include: the expression
    /// whose assertion failed, the name of the source file, and the line
    /// number where it happened. A usual expression format is:
    ///
    /// Assertion failed: expression, file filename, line line number
    /// This macro is disabled if at the moment of including assert.h a macro
    /// with the name NDEBUG has already been defined. This allows for a coder
    /// to include many assert calls in a source code while debugging the
    /// program and then disable all of them for the production version by
    /// simply including a line like:
    ///
    /// #define NDEBUG
    ///
    ///	at the beginning of its code, before the inclusion of assert.h.
    ///	Therefore, this macro is designed to capture programming errors, not
    ///	user or running errors, since it is generally disabled after a program
    ///	exits its debugging phase.
    ///
    /// Parameters
    ///
    /// expression
    ///		Expression to be evaluated. If this expression evaluates to 0,
    ///		this causes an assertion failure that terminates the program.
    /// </summary>
    /// <param name="condition"></param>
    [Conditional("DEBUG")]
    public static void assert(Boolean condition)
    {
        Debug.Assert(condition);
    }

    [Conditional("DEBUG")]
    public static void assert(Boolean condition, String message)
    {
        Debug.Assert(condition, message);
    }
}
