namespace stdc;

using System;

public partial class C
{

    #region Trigonometric functions
    public static double acos(double x)
    {
        return Math.Acos(x);
    }
    public static double asin(double x)
    {
        return Math.Asin(x);
    }
    public static double atan(double x)
    {
        return Math.Atan(x);
    }
    public static double atan2(double y, double x)
    {
        return Math.Atan2(y, x);
    }
    public static double cos(double x)
    {
        return Math.Cos(x);
    }
    public static double cosh(double x)
    {
        return Math.Cosh(x);
    }
    public static double sin(double x)
    {
        return Math.Sin(x);
    }
    public static double sinh(double x)
    {
        return Math.Sinh(x);
    }
    public static double tan(double x)
    {
        return Math.Tan(x);
    }
    public static double tanh(double x)
    {
        return Math.Tanh(x);
    }
    #endregion

    #region Rounding functions
    public static double ceil(double x)
    {
        return Math.Ceiling(x);
    }
    public static double floor(double x)
    {
        return Math.Floor(x);
    }
    #endregion

    #region Power functions
    public static double pow(double x, double y)
    {
        return Math.Pow(x, y);
    }
    public static double sqrt(double x)
    {
        double ret = Math.Sqrt(x);
        errno = double.IsNaN(ret) ? EDOM : 0;
        return ret;
    }
    #endregion

    #region Logarithmic functions
    public static double log(double x)
    {
        double ret = Math.Log(x);
        errno =
            double.IsNaN(ret) ? EDOM :
            double.IsNegativeInfinity(ret) ? ERANGE :
            0;
        return ret;
    }
    public static double log10(double x)
    {
        double ret = Math.Log10(x);
        errno =
            double.IsNaN(ret) ? EDOM :
            double.IsNegativeInfinity(ret) ? ERANGE :
            0;
        return ret;
    }
    public static double exp(double x)
    {
        return Math.Exp(x);
    }
    #endregion

    public static double fabs(double x)
    {
        return Math.Abs(x);
    }

    //public static double fmod(double, double);
    ////public static double frexp(double, int *);
    //public static double ldexp(double x, int y);

    //public static double modf(double, double *);

}
