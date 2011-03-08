using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace stdc {

	public static partial class C {

		#region FILE

		public class FILE {
			internal FILE (FileStream stream)
			{
				_error = 0;
				_stream = stream;
				if (_stream.CanRead)
					_reader = new StreamReader (_stream, Encoding.ASCII);
				if (_stream.CanWrite)
					_writer = new StreamWriter (_stream, Encoding.ASCII);
			}

			internal FILE (TextReader reader)
			{
				_reader = reader;
				_writer = null;
			}

			internal FILE (TextWriter writer)
			{
				_reader = null;
				_writer = writer;
			}

			private FileStream _stream;
			internal Boolean _tempFile;
			internal TextReader _reader;
			internal TextWriter _writer;
			internal int _error;
			internal Boolean _eof;
			internal String Name
			{
				get { return _stream.Name; }
			}

			internal void Flush ()
			{
				_stream.Flush (true);
			}

			internal int Close ()
			{
				String path = null;
				if (_tempFile) {
					path = _stream.Name;
				}
				_reader = null;
				if (_writer != null)
					_writer.Flush();
				_writer = null;
				_stream.Close ();
				_stream = null;
				if (_tempFile) {
					File.Delete (path);
				}
				return 0;
			}
		}

		#endregion

		//#define EOF (-1)
		public static readonly int EOF = -1;

		//#define stdin   (&_streams[0])
		public static readonly FILE stdin = new FILE (Console.In);
		//#define stdout  (&_streams[1])
		public static readonly FILE stdout = new FILE (Console.Out);
		//#define stderr  (&_streams[2])
		public static readonly FILE stderr = new FILE (Console.Error);

		#region SCANF functions (fscanf, scanf, sscanf)

		#region fscanf variants
		public static int fscanf<T> (TextReader stream, string format, out T p1)
		{
			string input = stream.ReadLine ();
			return sscanf (input, format, out p1);
		}
		public static int fscanf (TextReader stream, string format, out object p1, out object p2)
		{
			string input = stream.ReadLine ();
			return sscanf (input, format, out p1, out p2);
		}
		public static int fscanf (TextReader stream, string format, out object p1, out object p2, out object p3)
		{
			string input = stream.ReadLine ();
			return sscanf (input, format, out p1, out p2, out p3);
		}
		public static int fscanf (TextReader stream, string format, out object p1, out object p2, out object p3, out object p4)
		{
			string input = stream.ReadLine ();
			return sscanf (input, format, out p1, out p2, out p3, out p4);
		}
		public static int fscanf (TextReader stream, string format, out object p1, out object p2, out object p3, out object p4, out object p5)
		{
			string input = stream.ReadLine ();
			return sscanf (input, format, out p1, out p2, out p3, out p4, out p5);
		}
		#endregion

		//int  scanf ( const char * format, ... );

		//Read formatted data from stdin
		//Reads data from stdin and stores them according to the parameter format into the locations pointed by the additional arguments. The additional arguments should point to already allocated objects of the type specified by their corresponding format tag within the format string.

		//Parameters

		//format
		//    C string that contains one or more of the following items:

		//            * Whitespace character: the function will read and ignore any whitespace characters (this includes blank spaces and the newline and tab characters) which are encountered before the next non-whitespace character. This includes any quantity of whitespace characters, or none.
		//            * Non-whitespace character, except percentage signs (%): Any character that is not either a whitespace character (blank, newline or tab) or part of a format specifier (which begin with a % character) causes the function to read the next character from stdin, compare it to this non-whitespace character and if it matches, it is discarded and the function continues with the next character of format. If the character does not match, the function fails, returning and leaving subsequent characters of stdin unread.
		//            * Format specifiers: A sequence formed by an initial percentage sign (%) indicates a format specifier, which is used to specify the type and format of the data to be retrieved from stdin and stored in the locations pointed by the additional arguments. A format specifier follows this prototype:

		//              %[*][width][modifiers]type

		//              where:

		//              *	An optional starting asterisk indicates that the data is to be retrieved from stdin but ignored, i.e. it is not stored in the corresponding argument.
		//              width	Specifies the maximum number of characters to be read in the current reading operation
		//              modifiers	Specifies a size different from int (in the case of d, i and n), unsigned int (in the case of o, u and x) or float (in the case of e, f and g) for the data pointed by the corresponding additional argument:
		//              h : short int (for d, i and n), or unsigned short int (for o, u and x)
		//              l : long int (for d, i and n), or unsigned long int (for o, u and x), or double (for e, f and g)
		//              L : long double (for e, f and g)
		//              type	A character specifying the type of data to be read and how it is expected to be read. See next table.


		//              scanf type specifiers:
		//              type	Qualifying Input	Type of argument
		//              c	Single character: Reads the next character. If a width different from 1 is specified, the function reads width characters and stores them in the successive locations of the array passed as argument. No null character is appended at the end.	char *
		//              d	Decimal integer: Number optionally preceded with a + or - sign.	int *
		//              e,E,f,g,G	Floating point: Decimal number containing a decimal point, optionally preceded by a + or - sign and optionally folowed by the e or E character and a decimal number. Two examples of valid entries are -732.103 and 7.12e4	float *
		//              o	Octal integer.	int *
		//              s	String of characters. This will read subsequent characters until a whitespace is found (whitespace characters are considered to be blank, newline and tab).	char *
		//              u	Unsigned decimal integer.	unsigned int *
		//              x,X	Hexadecimal integer.	int *

		//additional arguments
		//    The function expects a sequence of references as additional arguments, each one pointing to an object of the type specified by their corresponding %-tag within the format string, in the same order.
		//    For each format specifier in the format string that retrieves data, an additional argument should be specified.
		//    These arguments are expected to be references (pointers): if you want to store the result of a fscanf operation on a regular variable you should precede its identifier with the reference operator, i.e. an ampersand sign (&), like in:

		//        int n;
		//        scanf ("%d",&n);

		//Return Value
		//On success, the function returns the number of items succesfully read. This count can match the expected number of readings or fewer, even zero, if a matching failure happens.
		//In the case of an input failure before any data could be successfully read, EOF is returned.
		#region scanf variants
		public static int scanf<T> (string format, out T p1)
		{
			return fscanf (Console.In, format, out p1);
		}
		public static int scanf (string format, out object p1, out object p2)
		{
			return fscanf (Console.In, format, out p1, out p2);
		}
		public static int scanf (string format, out object p1, out object p2, out object p3)
		{
			return fscanf (Console.In, format, out p1, out p2, out p3);
		}
		public static int scanf (string format, out object p1, out object p2, out object p3, out object p4)
		{
			return fscanf (Console.In, format, out p1, out p2, out p3, out p4);
		}
		public static int scanf (string format, out object p1, out object p2, out object p3, out object p4, out object p5)
		{
			return fscanf (Console.In, format, out p1, out p2, out p3, out p4, out p5);
		}
		#endregion

		#region sscanf variants
		public static int sscanf<T> (string input, string format, out T p1)
		{
			List<object> results;
			int count = ScanfHelper.Parse (input, format, out results);
			if (typeof(T) == typeof(Char[]))
				p1 = (T)(object)(((string)results[0]).ToCharArray());
			else
				p1 = (T)results[0];
			return count;
		}
		public static int sscanf (string input, string format, out object p1, out object p2)
		{
			List<object> results;
			int count = ScanfHelper.Parse (input, format, out results);
			p1 = results[0];
			p2 = results[1];
			return count;
		}
		public static int sscanf (string input, string format, out object p1, out object p2, out object p3)
		{
			List<object> results;
			int count = ScanfHelper.Parse (input, format, out results);
			p1 = results[0];
			p2 = results[1];
			p3 = results[2];
			return count;
		}
		public static int sscanf (string input, string format, out object p1, out object p2, out object p3, out object p4)
		{
			List<object> results;
			int count = ScanfHelper.Parse (input, format, out results);
			p1 = results[0];
			p2 = results[1];
			p3 = results[2];
			p4 = results[3];
			return count;
		}
		public static int sscanf (string input, string format, out object p1, out object p2, out object p3, out object p4, out object p5)
		{
			List<object> results;
			int count = ScanfHelper.Parse (input, format, out results);
			p1 = results[0];
			p2 = results[1];
			p3 = results[2];
			p4 = results[3];
			p5 = results[4];
			return count;
		}
		#endregion


		#endregion

		#region PRINTF functions (fprintf, printf, sprintf)

		public static int fprintf (TextWriter stream, string format, params object[] parameters)
		{
			String result;
			int count = sprintf (out result, format, parameters);
			stream.Write (result);
			return count;
		}

		public static int printf (string format, params object[] parameters)
		{
			return fprintf (Console.Out, format, parameters);
		}

		/// <summary>
		/// Writes into the string result a string consisting on a sequence of data formatted
		/// as the format argument specifies. After the format parameter, the function expects at least
		/// as many additional arguments as specified in format. This function behaves exactly as printf
		/// does, but writing its results to a string instead of stdout.
		/// 
		/// The syntax for a format placeholder is "%[parameter][flags][width][.precision][length]type".
		///
		/// Parameter can be omitted or can be:
		///   Character	Description
		///     n$      n is the number of the parameter to display using this format specifier, allowing
		///             the parameters provided to be output multiple times, using varying format specifiers
		///             or in different orders. This is a POSIX extension and not in C99. Example:
		///             printf("%2$d %1$#x %1$d",16,17) produces "17 0x10 16"
		///             
		/// Flags can be zero or more (in any order) of:
		///   Character	Description
		///   a number	Causes printf to left-pad the output with spaces until the required length of output
		///             is attained. If combined with '0' (see below), it will cause the sign to become a
		///             space when positive, but the remaining characters will be zero-padded
		///   +	        Causes printf to always denote the sign '+' or '-' of a number (the default is to
		///             omit the sign for positive numbers). Only applicable to numeric types.
		///   -	        Causes printf to left-align the output of this placeholder (the default is to
		///             right-align the output).
		///   #	        Alternate form. For 'g' and 'G', trailing zeros are not removed. For 'f', 'F', 'e',
		///             'E', 'g', 'G', the output always contains a decimal point. For 'o', 'x', and 'X',
		///             a 0, 0x, and 0X, respectively, is prepended to non-zero numbers.
		///   0	        Causes printf to use 0 instead of spaces to left-fill a fixed-length field. For 
		///             example, printf("%2d", 3) results in " 3", while printf("%02d", 3) results in "03".
		///             
		/// Width can be omitted or be any of:
		///   Character	Description
		///   a number	Causes printf to pad the output of this placeholder with spaces until it is at least
		///             number characters wide. As mentioned above, if number has a leading '0', that is
		///             interpreted as a flag, and the padding is done with '0' characters instead of spaces.
		///   *	        Causes printf to pad the output until it is n characters wide, where n is an integer
		///             value stored in the a function argument just preceding that represented by the
		///             modified type. For example printf("%*d", 5, 10) will result in "10" being printed
		///             with a width of 5.
		///             
		/// Precision can be omitted or be any of:
		///   Character	Description
		///   a number	For non-integral numeric types, causes the decimal portion of the output to be
		///             expressed in at least number digits. For the string type, causes the output to be
		///             truncated at number characters. If the precision is zero, nothing is printed for
		///             the corresponding argument.
		///   *	        Same as the above, but uses an integer value in the intaken argument to determine
		///             the number of decimal places or maximum string length. For example, printf("%.*s",
		///             3, "abcdef") will result in "abc" being printed.
		///             
		/// Length can be omitted or be any of:
		///   Character	Description
		///   hh	    For integer types, causes printf to expect an int sized integer argument which was
		///             promoted from a char.
		///   h	        For integer types, causes printf to expect a int sized integer argument which was
		///             promoted from a short.
		///   l	        For integer types, causes printf to expect a long sized integer argument.
		///   ll	    For integer types, causes printf to expect a long long sized integer argument.
		///   L	        For floating point types, causes printf to expect a long double argument.
		///   z	        For integer types, causes printf to expect a size_t sized integer argument.
		///   j	        For integer types, causes printf to expect a intmax_t sized integer argument.
		///   t	        For integer types, causes printf to expect a ptrdiff_t sized integer argument.
		///   
		/// Additionally, several platform specific length options came to exist prior to widespread use
		/// of the ISO C99 extensions:
		///   Character	Description
		///   I	        For signed integer types, causes printf to expect ptrdiff_t sized integer argument;
		///             for unsigned integer types, causes printf to expect size_t sized integer argument.
		///             Commonly found in Win32/Win64 platforms.
		///   I32	    For integer types, causes printf to expect a 32-bit (double word) integer argument.
		///             Commonly found in Win32/Win64 platforms.
		///   I64	    For integer types, causes printf to expect a 64-bit (quad word) integer argument.
		///             Commonly found in Win32/Win64 platforms.
		///   q	        For integer types, causes printf to expect a 64-bit (quad word) integer argument.
		///             Commonly found in BSD platforms.
		///             
		/// ISO C99 includes the inttypes.h header file that includes a number of macros for use in platform-independent printf coding. Example macros include:
		///   Character	Description
		///   "PRId32"	Typically equivalent to I32d (Win32/Win64) or d
		///   "PRId64"	Typically equivalent to I64d (Win32/Win64), lld (32-bit platforms) or ld (64-bit platforms)
		///   "PRIi32"	Typically equivalent to I32i (Win32/Win64) or i
		///   "PRIi64"	Typically equivalent to I64i (Win32/Win64), lli (32-bit platforms) or li (64-bit platforms)
		///   "PRIu32"	Typically equivalent to I32u (Win32/Win64) or u
		///   "PRIu64"	Typically equivalent to I64u (Win32/Win64), llu (32-bit platforms) or lu (64-bit platforms)
		///   
		/// Type can be any of:
		///   Character	Description
		///   d, i	    Print an int as a signed decimal number. '%d' and '%i' are synonymous for output, but are different when used with scanf() for input.
		///   u	        Print decimal unsigned int.
		///   f, F	    Print a double in normal (fixed-point) notation. 'f' and 'F' only differs in how the strings for an infinite number or NaN are printed ('inf', 'infinity' and 'nan' for 'f', 'INF', 'INFINITY' and 'NAN' for 'F').
		///   e, E	    Print a double value in standard form ([-]d.ddd e[+/-]ddd).An E conversion uses the letter E (rather than e) to introduce the exponent. The exponent always contains at least two digits; if the value is zero, the exponent is 00.
		///   g, G	    Print a double in either normal or exponential notation, whichever is more appropriate for its magnitude. 'g' uses lower-case letters, 'G' uses upper-case letters. This type differs slightly from fixed-point notation in that insignificant zeroes to the right of the decimal point are not included. Also, the decimal point is not included on whole numbers.
		///   x, X	    Print an unsigned int as a hexadecimal number. 'x' uses lower-case letters and 'X' uses upper-case.
		///   o	        Print an unsigned int in octal.
		///   s	        Print a character string.
		///   c	        Print a char (character).
		///   p	        Print a void * (pointer to void) in an implementation-defined format.
		///   n	        Print nothing, but write number of characters successfully written so far into an integer pointer parameter.
		///   %	        Print a literal '%' character (this type doesn't accept any flags, width, precision or length).
		/// </summary>
		/// <param name="result"></param>
		/// <param name="format"></param>
		/// <param name="parameters"></param>
		/// <returns>On success, the total number of characters written is returned. This count does
		/// not include the additional null-character automatically appended at the end of the string.
		/// On failure, a negative number is returned.</returns>
		public static int sprintf (out String result, string format, params object[] parameters)
		{
			return PrintfHelper.Output (out result, format, parameters);
		}
		#endregion

		#region FGET functions (fgetc, fgets)

		/// <summary>
		///		int fgetc ( FILE * stream );
		///		
		/// Get character from stream
		/// Returns the character currently pointed by the internal file position
		/// indicator of the specified stream. The internal file position indicator
		/// is then advanced by one character to point to the next character.
		/// 
		/// fgetc and getc are equivalent, except that the latter one may be
		/// implemented as a macro.
		/// 
		/// Parameters
		///  stream
		///   Pointer to a FILE object that identifies the stream on which the
		///   operation is to be performed.
		/// 
		/// Return Value
		///  The character read is returned as an int value.
		///  If the End-of-File is reached or a reading error happens, the function
		///  returns EOF and the corresponding error or eof indicator is set. You can
		///  use either ferror or feof to determine whether an error happened or the
		///  End-Of-File was reached.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static int fgetc (FILE stream)
		{
			return stream._reader.Read ();
		}

		/// <summary>
		///		int getc ( FILE * stream );
		///		
		/// Get character from stream
		/// Returns the character currently pointed by the internal file position
		/// indicator of the specified stream. The internal file position indicator
		/// is then advanced by one character to point to the next character.
		/// 
		/// getc is equivalent to fgetc and also expects a stream as parameter, but
		/// getc may be implemented as a macro, so the argument passed to it should
		/// not be an expression with potential side effects.
		/// 
		/// See getchar for a similar function without stream parameter.
		/// 
		/// Parameters
		///  stream
		///   pointer to a FILE object that identifies the stream on which the
		///   operation is to be performed.
		///   
		/// Return Value
		///  The character read is returned as an int value.
		///  If the End-of-File is reached or a reading error happens, the function
		///  returns EOF and the corresponding error or eof indicator is set. You can
		///  use either ferror or feof to determine whether an error happened or the
		///  End-Of-File was reached.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static int getc (FILE stream)
		{
			return fgetc (stream);
		}

		//int  ungetc(int , FILE *);
		//char *fgets(char *, int , FILE *);
		#endregion

		#region FPUT functions (fputc, fputs, putc, puts)

		/// <summary>
		///		int fputc ( int character, FILE * stream );
		///	
		/// Write character to stream
		///	Writes a character to the stream and advances the position indicator.
		///	The character is written at the current position of the stream as
		///	indicated by the internal position indicator, which is then advanced
		///	one character.
		///	
		/// Parameters
		///  character
		///   Character to be written. The character is passed as its int promotion.
		/// 
		///  stream
		///   Pointer to a FILE object that identifies the stream where the
		///   character is to be written. 
		///  
		/// Return Value
		///  If there are no errors, the same character that has been written is
		///  returned.
		///  If an error occurs, EOF is returned and the error indicator is set
		///  (see ferror).
		/// </summary>
		/// <param name="character"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static int fputc (int character, FILE stream)
		{
			stream._writer.Write ((char)character);
			return character;
		}

		/// <summary>
		///		int putc ( int character, FILE * stream );
		///	
		/// Write character to stream
		/// Writes a character to the stream and advances the position indicator.
		/// The character is written at the current position of the stream as
		/// indicated by the internal position indicator, which is then advanced
		/// one character.
		/// 
		/// putc is equivalent to fputc and also expects a stream as parameter,
		/// but putc may be implemented as a macro, so the argument passed should
		/// not be an expression with potential side effects.
		/// 
		/// See putchar for a similar function without stream parameter.
		/// 
		/// Parameters
		/// 
		/// character
		///  Character to be written. The character is passed as its int promotion.
		///
		/// stream
		///  Pointer to a FILE object that identifies the stream where the character
		///  is to be written. 
		///  
		/// Return Value
		///  If there are no errors, the same character that has been written is
		///  returned.
		///  If an error occurs, EOF is returned and the error indicator is set.
		/// </summary>
		/// <param name="character"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static int putc (int character, FILE stream)
		{
			return fputc (character, stream);
		}

		/// <summary>
		///		int fputs ( const char * str, FILE * stream );
		///		
		/// Write string to stream
		/// Writes the string pointed by str to the stream.
		/// 
		/// The function begins copying from the address specified (str) until
		/// it reaches the terminating null character ('\0'). This final null-
		/// character is not copied to the stream.
		/// 
		/// Parameters
		///  str
		///   An array containing the null-terminated sequence of characters
		///   to be written.
		///   
		///  stream
		///   Pointer to a FILE object that identifies the stream where the
		///   string is to be written.
		///   
		/// Return Value
		///  On success, a non-negative value is returned.
		///  On error, the function returns EOF.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="stream"></param>
		public static int fputs (string str, FILE stream)
		{
			stream._writer.Write (str);
			return str.Length;
		}

		/// <summary>
		///		int puts ( const char * str );
		/// 
		/// Write string to stdout
		/// Writes the C string pointed by str to stdout and appends a newline
		/// character ('\n').
		/// 
		/// The function begins copying from the address specified (str) until
		/// it reaches the terminating null character ('\0'). This final null-
		/// character is not copied to stdout.
		/// 
		/// Using fputs(str,stdout) instead, performs the same operation as
		/// puts(str) but without appending the newline character at the end.
		/// 
		/// Parameters
		///  str
		///   C string to be written.
		///   
		/// Return Value
		///  On success, a non-negative value is returned.
		///  On error, the function returns EOF.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int puts (string str)
		{
			fputs (str, stdout);
			fputs ("\n", stdout);
			return str.Length + 1;
		}

		public static int puts (StringBuilder str)
		{
			return puts (str.ToString ());
		}

		public static int puts (char[] str)
		{
			return puts (new string (str, 0, strlen (str)));
		}

		#endregion

		#region fopen, freopen, fclose

		/// <summary>
		/// FILE * fopen ( const char * filename, const char * mode );
		/// 
		/// Open file
		/// Opens the file whose name is specified in the parameter filename and
		/// associates it with a stream that can be identified in future
		/// operations by the FILE object whose pointer is returned. The
		/// operations that are allowed on the stream and how these are performed
		/// are defined by the mode parameter.
		/// 
		/// The running environment supports at least FOPEN_MAX files open
		/// simultaneously; FOPEN_MAX is a macro constant defined in cstdio.
		/// 
		/// Parameters
		/// filename
		///   C string containing the name of the file to be opened. This paramenter
		///   must follow the file name specifications of the running environment and
		///   can include a path if the system supports it.
		///   
		/// mode
		///   C string containing a file access modes. It can be:
		///   - "r"	 Open a file for reading. The file must exist.
		///   - "w"	 Create an empty file for writing. If a file with the same name
		///          already exists its content is erased and the file is treated as
		///          a new empty file.
		///   - "a"	 Append to a file. Writing operations append data at the end of the
		///          file. The file is created if it does not exist.
		///   - "r+" Open a file for update both reading and writing. The file must exist.
		///   - "w+" Create an empty file for both reading and writing. If a file with the
		///          same name already exists its content is erased and the file is
		///          treated as a new empty file.
		///   - "a+" Open a file for reading and appending. All writing operations are
		///          performed at the end of the file, protecting the previous content to
		///          be overwritten. You can reposition (fseek, rewind) the internal
		///          pointer to anywhere in the file for reading, but writing operations
		///          will move it back to the end of file. The file is created if it does
		///          not exist.
		///          
		/// With the mode specifiers above the file is open as a text file. In order to open
		/// a file as a binary file, a "b" character has to be included in the mode string.
		/// This additional "b" character can either be appended at the end of the string
		/// (thus making the following compound modes: "rb", "wb", "ab", "r+b", "w+b", "a+b")
		/// or be inserted between the letter and the "+" sign for the mixed modes ("rb+",
		/// "wb+", "ab+").
		/// 
		/// Additional characters may follow the sequence, although they should have no effect.
		/// For example, "t" is sometimes appended to make explicit the file is a text file.
		/// 
		/// In the case of text files, depending on the environment where the application runs,
		/// some special character conversion may occur in input/output operations to adapt
		/// them to a system-specific text file format. In many environments, such as most
		/// UNIX-based systems, it makes no difference to open a file as a text file or a
		/// binary file; Both are treated exactly the same way, but differentiation is
		/// recommended for a better portability.
		/// 
		/// For the modes where both read and writing (or appending) are allowed (those which
		/// include a "+" sign), the stream should be flushed (fflush) or repositioned (fseek,
		/// fsetpos, rewind) between either a reading operation followed by a writing operation
		/// or a writing operation followed by a reading operation.
		/// 
		/// Return Value
		/// If the file has been succesfully opened the function will return a pointer to a
		/// FILE object that is used to identify the stream on all further operations involving
		/// it. Otherwise, a null pointer is returned.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static FILE fopen (string filename, string mode)
		{
			try {
				if (string.IsNullOrEmpty (mode))
					throw new InvalidOperationException ("invalid mode");
				FileAccess a;
				FileMode m;
				switch (mode) {
				case "r":
				case "rt":
				case "rb":
					a = FileAccess.Read;
					m = FileMode.Open;
					break;
				case "w":
				case "wt":
				case "wb":
					a = FileAccess.Write;
					m = FileMode.Create;
					break;
				case "a":
				case "at":
				case "ab":
					a = FileAccess.Write;
					m = FileMode.Append;
					break;
				case "r+":
				case "r+t":
				case "rb+":
				case "r+b":
					a = FileAccess.ReadWrite;
					m = FileMode.Open;
					break;
				case "w+":
				case "w+t":
				case "wb+":
				case "w+b":
					a = FileAccess.ReadWrite;
					m = FileMode.Create;
					break;
				case "a+":
				case "a+t":
				case "ab+":
				case "a+b":
					a = FileAccess.ReadWrite;
					m = FileMode.Append;
					break;
				default:
					throw new InvalidOperationException ("invalid mode (2)");
				}
				FileStream stream = new FileStream (filename, m, a, FileShare.Read);
				FILE file = new FILE (stream);
				return file;
			}
			catch (FileNotFoundException ex) {
				errno = ENOENT;
			}
			catch (InvalidOperationException ex) {
				errno = EINVAL;
			}
			catch (Exception ex) {
				errno = EIO;
			}
			return null;
		}

		//FILE *freopen(const char *, const char *, FILE *);
		//Reopen stream with different file or mode
		//freopen first tries to close any file already associated with the stream given as third parameter and disassociates it.
		//Then, whether that stream was successfuly closed or not, freopen opens the file whose name is passed in the first parameter, filename, and associates it with the specified stream just as fopen would do using the mode value specified as the second parameter.
		//This function is specially useful for redirecting predefined streams like stdin, stdout and stderr to specific files (see the example below).

		//Parameters

		//filename
		//    C string containing the name of the file to be opened. This parameter must follow the file specifications of the running environment and can include a path if the system supports it.
		//    If this parameter is a null pointer, the function attemps to change the mode of the stream specified as third parater to the one specified in the mode parameter, as if the file name currently associated with that stream had been used.
		//mode
		//    C string containing the file access modes. It can be:
		//    "r"	Open a file for reading. The file must exist.
		//    "w"	Create an empty file for writing. If a file with the same name already exists its content is erased and the file is treated as a new empty file.
		//    "a"	Append to a file. Writing operations append data at the end of the file. The file is created if it does not exist.
		//    "r+"	Open a file for update both reading and writing. The file must exist.
		//    "w+"	Create an empty file for both reading and writing. If a file with the same name already exists its content is erased and the file is treated as a new empty file.
		//    "a+"	Open a file for reading and appending. All writing operations are performed at the end of the file, protecting the previous content to be overwritten. You can reposition (fseek, rewind) the internal pointer to anywhere in the file for reading, but writing operations will move it back to the end of file. The file is created if it does not exist.

		//    An additional b is used to specify the file is to be reopened in binary mode. For more details on these modes, refer to fopen.
		//stream
		//    pointer to a FILE object that identifies the stream to be reopened.

		//Return Value
		//If the file was succesfully reopened, the function returns a pointer to an object identifying the stream. Otherwise, a null pointer is returned. 

		/// <summary>
		///		int fclose(FILE * stream);
		///		
		///	Close file
		/// Closes the file associated with the stream and disassociates it.
		/// All internal buffers associated with the stream are flushed: the
		/// content of any unwritten buffer is written and the content of any
		/// unread buffer is discarded.
		/// Even if the call fails, the stream passed as parameter will no
		/// longer be associated with the file.
		/// </summary>
		/// <param name="f">Pointer to a FILE object that specifies the stream to be closed. </param>
		/// <returns>If the stream is successfully closed, a zero value is returned. On failure, EOF is returned.</returns>
		public static int fclose (FILE stream)
		{
			return stream.Close ();
		}
		#endregion

		public const int SEEK_CUR    = 1;
		public const int SEEK_END    = 2;
		public const int SEEK_SET    = 0;

		/// <summary>
		/// void clearerr ( FILE * stream );
		/// 
		/// Clear error indicators
		/// Resets both the error and the EOF indicators of the stream.
		/// When a stream function fails either because of an error or because the end
		/// of the file has been reached, one of these internal indicators may be set.
		/// These indicators remain set until either this, rewind, fseek or fsetpos is
		/// called.
		/// 
		/// Parameters
		/// stream
		///		Pointer to a FILE object that identifies the stream.
		///	
		/// Return Value
		///	None
		/// </summary>
		/// <param name="file"></param>
		public static void clearerr (FILE file)
		{
			file._error = 0;
			file._eof = false;
		}

		/// <summary>
		/// int ferror ( FILE * stream );
		/// 
		/// Check error indicator
		/// Checks if the error indicator associated with stream is set, returning a
		/// value different from zero if it is.
		/// 
		/// This indicator is generaly set by a previous operation on the stream that
		/// failed.
		/// 
		/// Parameters
		/// stream
		///		Pointer to a FILE object that identifies the stream. 
		///		
		/// Return Value
		///		If the error indicator associated with the stream was set, the function
		///		returns a nonzero value. Otherwise, it returns a zero value.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static int ferror (FILE file)
		{
			return file._error;
		}

		/// <summary>
		/// int feof ( FILE * stream );
		/// 
		/// Check End-of-File indicator
		/// Checks whether the End-of-File indicator associated with stream is set,
		/// returning a value different from zero if it is.
		/// This indicator is generally set by a previous operation on the stream that
		/// reached the End-of-File.
		/// Further operations on the stream once the End-of-File has been reached will
		/// fail until either rewind, fseek or fsetpos is successfully called to set the
		/// position indicator to a new value.
		/// 
		/// Parameters
		/// stream
		///		Pointer to a FILE object that identifies the stream.
		///		
		/// Return Value
		/// A non-zero value is returned in the case that the End-of-File indicator
		/// associated with the stream is set. Otherwise, a zero value is returned.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static Boolean feof (FILE file)
		{
			return file._eof;
		}

		/// <summary>
		/// int fflush ( FILE * stream );
		/// 
		/// Flush stream
		/// If the given stream was open for writing and the last i/o operation was
		/// an output operation, any unwritten data in the output buffer is written
		/// to the file.
		/// If it was open for reading and the last operation was an input operation,
		/// the behavior depends on the specific library implementation. In some
		/// implementations this causes the input buffer to be cleared, but this is
		/// not standard behavior.
		/// If the argument is a null pointer, all open files are flushed.
		/// The stream remains open after this call.
		/// When a file is closed, either because of a call to fclose or because the
		/// program terminates, all the buffers associated with it are automatically
		/// flushed.
		/// 
		/// Parameters
		/// stream
		///		Pointer to a FILE object that specifies a buffered stream.
		///		
		/// Return Value
		///		A zero value indicates success.
		///		If an error occurs, EOF is returned and the error indicator is set (see feof).
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static int fflush (FILE file)
		{
			try {
				file.Flush ();
				return 0;
			}
			catch (Exception ex) {
				//TODO: set errno accordingly
				return EOF;
			}
		}

		/// <summary>
		/// FILE * tmpfile ( void );
		/// 
		/// Open a temporary file
		/// Creates a temporary binary file, open for update (wb+ mode -- see fopen
		/// for details). The filename is guaranteed to be different from any other
		/// existing file.
		/// The temporary file created is automatically deleted when the stream is
		/// closed (fclose) or when the program terminates normally.
		/// 
		/// Parameters
		/// none
		/// 
		/// Return Value
		///		If successful, the function returns a stream pointer to the temporary
		///		file created.
		///		If the file cannot be created, NULL is returned.
		/// </summary>
		/// <returns></returns>
		public static FILE tmpfile ()
		{
			FILE f = fopen (Path.GetTempFileName (), "wb+");
			f._tempFile = true;
			return f;
		}

		/// <summary>
		/// char * tmpnam ( char * str );
		/// 
		/// Generate temporary filename
		/// A string containing a filename different from any existing file is generated.
		/// This string can be used to create a temporary file without overwriting any other existing file.
		/// If the str argument is a null pointer, the resulting string is stored in an internal static
		/// array that can be accessed by the return value. The content of this string is stored until a
		/// subsequent call to this same function erases it.
		/// If the str argument is not a null pointer, it must point to an array of at least L_tmpnam bytes
		/// that will be filled with the proposed tempname. L_tmpnam is a macro constant defined in <cstdio>.
		/// The file name returned by this function can be used to create a regular file using fopen to be
		/// used as a temp file. The file created this way, unlike those created with tmpfile is not
		/// automatically deleted when closed; You should call remove to delete this file once closed.
		/// 
		/// Parameters
		/// str
		///		Pointer to an array of chars where the proposed tempname will be stored as a C string.
		///		The size of this array should be at least L_tmpnam characters.
		///		Alternativelly, a null pointer can be specified, in which case the string will be stored
		///		in an internal static array that can be accessed with the return value.
		///		
		/// Return Value
		///		A pointer to the C string containing the proposed name for a temporary file.
		///		If str was a null pointer, this points to an internal buffer that will be overwritten the
		///		next time this function is called.
		///		If str was not a null pointer, str is returned.
		///		If the function fails to create a suitable filename, it returns a null pointer.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string tmpnam ()
		{
			// only null supported at the moment
			return Path.GetRandomFileName ();
		}

		//int  fgetpos(FILE *, fpos_t *);
		//int  fprintf(FILE *, const char *, ...);
		//size_t fread(void *, size_t , size_t , FILE *);
		//int  fscanf(FILE *, const char *, ...);
		//int  fseek(FILE *, long , int );
		//int  fsetpos(FILE *, const fpos_t *);
		//long ftell(FILE *);
		//size_t fwrite(const void *, size_t , size_t , FILE *);
		//char *gets(char *);

		/// <summary>
		///		void perror ( const char * str );
		/// 
		/// Print error message
		/// Interprets the value of the global variable errno into a string and prints
		/// that string to stderr (standard error output stream, usually the screen),
		/// optionaly preceding it with the custom message specified in str.
		/// 
		/// errno is an integral variable whose value describes the last error produced
		/// by a call to a library function. The error strings produced by perror
		/// depend on the developing platform and compiler.
		/// 
		/// If the parameter str is not a null pointer, str is printed followed by a
		/// colon (:) and a space. Then, whether str was a null pointer or not, the
		/// generated error description is printed followed by a newline character ('\n').
		/// 
		/// perror should be called right after the error was produced, otherwise it
		/// can be overwritten in calls to other functions.
		///
		/// Parameters.
		/// str
		///    C string containing a custom message to be printed before the error
		///    message itself.
		///    If it is a null pointer, no preceding custom message is printed, but the
		///    error message is printed anyway.
		///    By convention, the name of the application itself is generally used as
		///    parameter.
		/// </summary>
		/// <param name="str"></param>
		public static void perror (string str)
		{
			string errmsg = interpret_errno ();
			if (str == null)
				stderr._writer.WriteLine ("{0}", errmsg);
			else
				stderr._writer.WriteLine ("{0}: {1}", str, errmsg);
		}

		private static string interpret_errno ()
		{
			switch (errno) {
			//TODO: add the other values
			case EDOM:
				return "Math argument";
			case ERANGE:
				return "Result too large";
			case EINVAL:
				return "Invalid argument";
			case ENOENT:
				return "No such file or directory";
			default:
				errno = EINVAL;
				return String.Format ("Unknown error {0}", errno);
			}
		}

		/// <summary>
		/// int remove ( const char * filename );
		/// 
		/// Remove file
		/// Deletes the file whose name is specified in filename.
		/// This is an operation performed directly on a file; No streams are involved in the operation.
		/// 
		/// Parameters
		/// filename
		///		C string containing the name of the file to be deleted. This paramenter must follow the
		///		file name specifications of the running environment and can include a path if the system
		///		supports it.
		///		
		/// Return value
		///		If the file is successfully deleted, a zero value is returned.
		///		On failure, a nonzero value is reurned and the errno variable is set to the corresponding
		///		error code. Error codes are numerical values representing the type of failure occurred.
		///		A string interpreting this value can be printed to the standard error stream by a call
		///		to perror.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static int remove (string filename)
		{
			File.Delete (filename);
			return 0;
		}

		/// <summary>
		/// int rename ( const char * oldname, const char * newname );
		/// 
		/// Rename file
		/// Changes the name of the file or directory specified by oldname to newname.
		/// If oldname and newname specify different paths and this is supported by the system, the file
		/// is moved to the new location.
		/// This is an operation performed directly on a file; No streams are involved in the operation.
		/// 
		/// Parameters
		/// oldname
		///		C string containing the name of the file to be renamed and/or moved. This file must exist
		///		and the correct writing permissions should be available.
		///		
		/// newname
		///		C string containing the new name for the file. This shall not be the name of an existing
		///		file; if it is, the behavior to be expected depends on the running environment, which may
		///		either be failure or overriding.
		///		
		/// Return value
		///		If the file is successfully renamed, a zero value is returned.
		///		On failure, a nonzero value is returned and the errno variable is set to the corresponding
		///		error code. Error codes are numerical values representing the type of failure occurred. A
		///		string interpreting this value can be printed to the standard error stream by a call to perror.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static int rename (string oldname, string newname)
		{
			File.Move (oldname, newname);
			return 0;
		}

		//int  printf(const char *, ...);
		//void rewind(FILE *);
		//int  scanf(const char *, ...);
		//void setbuf(FILE *, char *);
		//int  setvbuf(FILE *, char *, int, size_t );
		//int  sprintf(char *, const char *, ...);
		//int  sscanf(const char *, const char *, ...);

		//int getchar(void);
		//int putchar(int);

		//#ifndef __VA_LIST
		//#define __VA_LIST
		//typedef char *va_list;
		//#endif

		//int  vfprintf(FILE *, const char *, va_list );
		//int  vprintf( const char *, va_list );
		//int  vsprintf(char *,  const char *, va_list );

		//#endif

		//typedef long    fpos_t;

		//#define _IOFBF  0
		//#define _IOLBF  1
		//#define _IONBF  2

		//#define FOPEN_MAX 8
		//#define FILENAME_MAX 100
		//#define BUFSIZ  256
		//#define L_tmpnam    12
		//#define TMP_MAX     25
		//extern  FILE    _streams[];
	}
}
