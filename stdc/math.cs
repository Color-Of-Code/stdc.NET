using System;
using System.Collections.Generic;
using System.Text;

namespace stdc {
	public static partial class c {

		#region Trigonometric functions
		public static double acos (double x)
		{
			return Math.Acos (x);
		}
		public static double asin (double x)
		{
			return Math.Asin (x);
		}
		public static double atan (double x)
		{
			return Math.Atan (x);
		}
		public static double atan2 (double y, double x)
		{
			return Math.Atan2 (y, x);
		}
		public static double cos (double x)
		{
			return Math.Cos (x);
		}
		public static double cosh (double x)
		{
			return Math.Cosh (x);
		}
		public static double sin (double x)
		{
			return Math.Sin (x);
		}
		public static double sinh (double x)
		{
			return Math.Sinh (x);
		}
		public static double tan (double x)
		{
			return Math.Tan (x);
		}
		public static double tanh (double x)
		{
			return Math.Tanh (x);
		}
		#endregion

		#region Rounding functions
		public static double ceil (double x)
		{
			return Math.Ceiling (x);
		}
		public static double floor (double x)
		{
			return Math.Floor (x);
		}
		#endregion

		#region Power functions
		public static double pow (double x, double y)
		{
			return Math.Pow (x, y);
		}
		public static double sqrt (double x)
		{
			return Math.Sqrt (x);
		}
		#endregion

		#region Logarithmic functions
		public static double log (double x)
		{
			return Math.Log (x);
		}
		public static double log10 (double x)
		{
			return Math.Log10 (x);
		}
		public static double exp (double x)
		{
			return Math.Exp (x);
		}
		#endregion

		//public static double fabs(double x);
		//public static double fmod(double, double);
		////public static double frexp(double, int *);
		//public static double ldexp(double x, int y);
		////public static double modf(double, double *);

	}
}
