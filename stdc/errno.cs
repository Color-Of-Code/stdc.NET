using System;
using System.Collections.Generic;
using System.Text;

namespace stdc {
	public static partial class C {

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
		/// The same header that declares errno (<cerrno>) also declares at least
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

	}
}
