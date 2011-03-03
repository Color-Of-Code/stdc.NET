﻿using System;
using System.Collections.Generic;
using System.Text;

namespace stdc {

	//#define EXIT_SUCCESS 0
	//#define EXIT_FAILURE 1
	public enum EXIT_STATUS {
		SUCCESS = 0,
		FAILURE = 1
	}


	public static partial class C {

		//#define NULL ((void *) 0)
		public static readonly object NULL = null;

		public static void exit (int code)
		{
			Environment.Exit (code);
		}

		public static void exit (EXIT_STATUS code)
		{
			exit ((int)code);
		}

		//void abort (void);
		public static void abort ()
		{
			throw new NotImplementedException ();
		}


		//typedef struct {
		// int quot;
		// int rem;
		//} div_t;

		//typedef struct {
		// long quot;
		// long rem;
		//} ldiv_t;

		//#define MB_CUR_MAX  _Mbcurmax

		public const int RAND_MAX = 0x7FFF;

		//int abs (int);
		public static int abs (int l)
		{
			return Math.Abs (l);
		}

		//typedef void (* __atexit_t)(void);
		public delegate void atexit_handler ();

		private static List<atexit_handler> _atexitHandlers = new List<atexit_handler> ();

		/// <summary>
		///		int atexit ( void ( * function ) (void) );
		///		
		/// Set function to be executed on exit
		/// The function pointed by the function pointer argument is called
		/// when the program terminates normally.
		/// 
		/// If more than one atexit function has been specified by different 
		/// calls to this function, they are all executed in reverse order as a stack,
		/// i.e. the last function specified is the first to be executed at exit.
		/// 
		/// One single function can be registered to be executed at exit more than once.
		/// 
		/// C++ implementations are required to support the registration of at least
		/// 32 atexit functions.
		/// 
		/// Parameters
		/// 
		/// function
		///		Function to be called. The function has to return no value and accept
		///		no arguments.
		///		
		/// Return Value
		///		A zero value is returned if the function was successfully registered,
		///		or a non-zero value if it failed.
		/// </summary>
		/// <param name="function"></param>
		/// <returns></returns>
		public static int atexit (atexit_handler function)
		{
			_atexitHandlers.Insert (0, function);
			return 0;
		}

		//double atof (const char *);
		public static double atof (string s)
		{
			return double.Parse (s);
		}

		//int atoi (const char *);
		public static int atoi (string s)
		{
			return int.Parse (s);
		}

		//long atol (const char *);
		public static long atol (string s)
		{
			return long.Parse (s);
		}

		//long labs (long);
		public static long labs (long l)
		{
			return Math.Abs (l);
		}

		//int rand (void);
		/// <summary>
		///		int rand ( void );
		///		
		/// Generate random number
		/// Returns a pseudo-random integral number in the range 0 to RAND_MAX.
		/// 
		/// This number is generated by an algorithm that returns a sequence of 
		/// apparently non-related numbers each time it is called. This algorithm
		/// uses a seed to generate the series, which should be initialized to
		/// some distinctive value using srand.
		/// 
		/// RAND_MAX is a constant defined in <cstdlib>. Its default value may vary
		/// between implementations but it is granted to be at least 32767.
		/// 
		/// A typical way to generate pseudo-random numbers in a determined range
		/// using rand is to use the modulo of the returned value by the range span
		/// and add the initial value of the range:
		///		( value % 100 ) is in the range 0 to 99
		///		( value % 100 + 1 ) is in the range 1 to 100
		///		( value % 30 + 1985 ) is in the range 1985 to 2014
		///		
		/// Notice though that this modulo operation does not generate a truly
		/// uniformly distributed random number in the span (since in most cases 
		/// lower numbers are slightly more likely), but it is generally a good
		/// approximation for short spans.
		/// 
		/// Parameters
		///		(none)
		///		
		/// Return Value
		///		An integer value between 0 and RAND_MAX. 
		/// </summary>
		/// <returns></returns>
		public static int rand ()
		{
			return _rg.Next (0, RAND_MAX);
		}

		private static Random _rg = new Random (1);

		/// <summary>
		///		void srand ( unsigned int seed );
		///		
		/// Initialize random number generator
		/// The pseudo-random number generator is initialized using the
		/// argument passed as seed.
		/// 
		/// For every different seed value used in a call to srand, the pseudo-
		/// random number generator can be expected to generate a different
		/// succession of results in the subsequent calls to rand.
		/// 
		/// Two different initializations with the same seed, instructs the
		/// pseudo-random generator to generate the same succession of results
		/// for the subsequent calls to rand in both cases.
		/// 
		/// If seed is set to 1, the generator is reinitialized to its initial 
		/// value and produces the same values as before any call to rand or srand.
		/// 
		/// In order to generate random-like numbers, srand is usually initialized
		/// to some distinctive value, like those related with the execution time.
		/// For example, the value returned by the function time (declared in 
		/// header <ctime>) is different each second, which is distinctive enough
		/// for most randoming needs.
		/// 
		/// Parameters
		///		seed
		///			An integer value to be used as seed by the pseudo-random number
		///			generator algorithm.
		///			
		/// Return Value
		///		(none)
		/// </summary>
		/// <param name="seed"></param>
		public static void srand (uint seed)
		{
			_rg = new Random ((int)seed);
		}


		//void * bsearch(const void *, const void *,
		//     size_t, size_t,
		//     int (*) (const void *, const void *));
		//div_t div (int, int);


		//char * getenv (const char *);
		public static string getenv (string name)
		{
			return Environment.GetEnvironmentVariable (name);
		}

		//ldiv_t ldiv (long, long);

		/// <summary>
		///		void qsort ( void * base, size_t num, size_t size, int ( * comparator ) ( const void *, const void * ) );
		///		
		/// Sort elements of array
		/// Sorts the num elements of the array pointed by base, each element 
		/// size bytes long, using the comparator function to determine the order.
		/// 
		/// The sorting algorithm used by this function compares pairs of values by
		/// calling the specified comparator function with two pointers to elements
		/// of the array.
		/// 
		/// The function does not return any value, but modifies the content of the
		/// array pointed by base reordering its elements to the newly sorted order.
		/// 
		/// Parameters
		/// base
		///		Pointer to the first element of the array to be sorted.
		///		
		/// num
		///		Number of elements in the array pointed by base.
		///		
		/// size
		///		Size in bytes of each element in the array.
		///		
		/// comparator
		///		Function that compares two elements. The function shall follow this prototype:
		///		    int comparator ( const void * elem1, const void * elem2 );
		///		    
		///		The function must accept two parameters that are pointers to elements, type-
		///		casted as void*. These parameters should be cast back to some data type and 
		///		be compared.
		///		The return value of this function should represent whether elem1 is considered
		///		less than, equal to, or greater than elem2 by returning, respectively, a
		///		negative value, zero or a positive value.
		///		
		/// Return Value
		///		(none) 
		/// </summary>
		[Obsolete ("This call can usually be replaced by the 2 parameter variant: parameters count and size are only used to provide a consistency check")]
		public static void qsort<T> (T[] b, int count, int size, Comparison<T> cp)
		{
			assert (count == b.Length);
			assert (size == System.Runtime.InteropServices.Marshal.SizeOf (typeof (T)));
			Array.Sort (b, cp);
		}

		public static void qsort<T> (T[] b, Comparison<T> cp)
		{
			Array.Sort (b, cp);
		}


		//double strtod (const char *, char **);
		//long  strtol (const char *, char **, int);
		//unsigned long strtoul (const char *, char **, int);

		//int system (const char *);

		//extern char _Mbcurmax;

		//extern int mblen(const char *, size_t);
		//extern int mbtowc(wchar_t *, const char *, size_t);
		//extern int wctomb(char *, wchar_t);
		//extern size_t mbstowcs(wchar_t *, const char *, size_t);
		//extern size_t wcstombs(char *, const wchar_t *, size_t);

		//void * calloc (size_t, size_t);
		//void free (void *);
		//void * malloc (size_t);
		//void * realloc(void *, size_t);
	}
}
