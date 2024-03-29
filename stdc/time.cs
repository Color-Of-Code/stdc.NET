﻿namespace stdc;

using System;

public struct tm
{
    public int tm_sec;         /* seconds after the minute - [0,59] */
    public int tm_min;         /* minutes after the hour - [0,59] */
    public int tm_hour;        /* hours since midnight - [0,23] */
    public int tm_mday;        /* day of the month - [1,31] */
    public int tm_mon;         /* months since January - [0,11] */
    public int tm_year;        /* years since 1900 */
    public int tm_wday;        /* days since Sunday - [0,6] */
    public int tm_yday;        /* days since January 1 - [0,365] */
    public int tm_isdst;       /* daylight savings time flag */
}

public struct time_t
{
    // seconds since January 1, 1970
    public uint _time_t;
}

public struct clock_t
{
    public long _clock_t;

    public static clock_t operator +(clock_t left, long add) => new clock_t() { _clock_t = left._clock_t + add };
    public static bool operator >=(clock_t left, clock_t right) => left._clock_t >= right._clock_t;
    public static bool operator <=(clock_t left, clock_t right) => left._clock_t <= right._clock_t;
}

public partial class C
{
    // Expands to an expression (not necessarily a compile-time constant) of type clock_t equal to the number of clock ticks per second, as returned by clock().
    public const long CLOCKS_PER_SEC = TimeSpan.TicksPerMillisecond;

    //char *asctime(const struct tm *);
    //char *ctime(const time_t *);

    // Returns the approximate processor time used by the process since the beginning of an implementation-defined era related to the program's execution.
    // To convert result value to seconds, divide it by CLOCKS_PER_SEC.
    public static clock_t clock()
    {
        return new clock_t() { _clock_t = DateTime.UtcNow.Ticks };
    }

    //double  difftime(time_t, time_t);
    //struct tm *gmtime(const time_t *);
    //struct tm *localtime(const time_t *);
    //time_t  mktime(struct tm *);
    //time_t  time(time_t *);
    /// <summary>
    ///		time_t time ( time_t * timer );
    ///
    /// Get current time
    /// Get the current calendar time as a time_t object.
    /// The function returns this value, and if the argument is not a
    /// null pointer, the value is also set to the object pointed by timer.
    ///
    /// Parameters
    ///
    /// timer
    ///		Pointer to an object of type time_t, where the time value is stored.
    ///		Alternativelly, this parameter can be a null pointer, in which case
    ///		the parameter is not used, but a time_t object is still returned by
    ///		the function.
    ///
    /// Return Value
    /// The current calendar time as a time_t object.
    /// If the argument is not a null pointer, the return value is the same as
    /// the one stored in the location pointed by the argument.
    ///
    /// If the function could not retrieve the calendar time, it returns a -1 value.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static uint time(time_t t)
    {
        uint val = (uint)(DateTime.Now - _time_origin).TotalSeconds;
        t._time_t = val;
        return val;
    }

    private static DateTime _time_origin = new DateTime(1970, 1, 1);

    public static uint time(object ignored)
    {
        uint val = (uint)(DateTime.Now - _time_origin).TotalSeconds;
        return val;
    }

    //size_t strftime(char *, size_t,
    //    const char *, const struct tm *);


}
