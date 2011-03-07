using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace stdc {

	public class FILE {
		internal FILE (FileStream stream)
		{
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
		internal TextReader _reader;
		internal TextWriter _writer;

		internal int Close ()
		{
			if (_reader != null) {
				_reader.Close ();
				_reader = null;
			}
			if (_writer != null) {
				_writer.Close ();
				_writer = null;
			}
			//_stream.Flush ();
			_stream.Close ();
			_stream = null;
			return 0;
		}
	}

	public static partial class C {
		#region Public Methods

		#region IsNumericType
		/// <summary>
		/// Determines whether the specified value is of numeric type.
		/// </summary>
		/// <param name="o">The object to check.</param>
		/// <returns>
		/// 	<c>true</c> if o is a numeric type; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsNumericType (object o)
		{
			return (o is byte || o is sbyte ||
				o is short || o is ushort ||
				o is int || o is uint ||
				o is long || o is ulong ||
				o is float ||
				o is double ||
				o is decimal);
		}
		#endregion
		#region IsPositive
		/// <summary>
		/// Determines whether the specified value is positive.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <param name="ZeroIsPositive">if set to <c>true</c> treats 0 as positive.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is positive; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsPositive (object Value, bool ZeroIsPositive)
		{
			switch (Type.GetTypeCode (Value.GetType ())) {
			case TypeCode.SByte:
				return (ZeroIsPositive ? (sbyte)Value >= 0 : (sbyte)Value > 0);
			case TypeCode.Int16:
				return (ZeroIsPositive ? (short)Value >= 0 : (short)Value > 0);
			case TypeCode.Int32:
				return (ZeroIsPositive ? (int)Value >= 0 : (int)Value > 0);
			case TypeCode.Int64:
				return (ZeroIsPositive ? (long)Value >= 0 : (long)Value > 0);
			case TypeCode.Single:
				return (ZeroIsPositive ? (float)Value >= 0 : (float)Value > 0);
			case TypeCode.Double:
				return (ZeroIsPositive ? (double)Value >= 0 : (double)Value > 0);
			case TypeCode.Decimal:
				return (ZeroIsPositive ? (decimal)Value >= 0 : (decimal)Value > 0);
			case TypeCode.Byte:
				return (ZeroIsPositive ? true : (byte)Value > 0);
			case TypeCode.UInt16:
				return (ZeroIsPositive ? true : (ushort)Value > 0);
			case TypeCode.UInt32:
				return (ZeroIsPositive ? true : (uint)Value > 0);
			case TypeCode.UInt64:
				return (ZeroIsPositive ? true : (ulong)Value > 0);
			case TypeCode.Char:
				return (ZeroIsPositive ? true : (char)Value != '\0');
			default:
				return false;
			}
		}
		#endregion
		#region ToUnsigned
		/// <summary>
		/// Converts the specified values boxed type to its correpsonding unsigned
		/// type.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <returns>A boxed numeric object whos type is unsigned.</returns>
		private static object ToUnsigned (object Value)
		{
			switch (Type.GetTypeCode (Value.GetType ())) {
			case TypeCode.SByte:
				return (byte)((sbyte)Value);
			case TypeCode.Int16:
				return (ushort)((short)Value);
			case TypeCode.Int32:
				return (uint)((int)Value);
			case TypeCode.Int64:
				return (ulong)((long)Value);

			case TypeCode.Byte:
				return Value;
			case TypeCode.UInt16:
				return Value;
			case TypeCode.UInt32:
				return Value;
			case TypeCode.UInt64:
				return Value;

			case TypeCode.Single:
				return (UInt32)((float)Value);
			case TypeCode.Double:
				return (ulong)((double)Value);
			case TypeCode.Decimal:
				return (ulong)((decimal)Value);

			default:
				return null;
			}
		}
		#endregion
		#region ToInteger
		/// <summary>
		/// Converts the specified values boxed type to its correpsonding integer
		/// type.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <returns>A boxed numeric object whos type is an integer type.</returns>
		private static object ToInteger (object Value, bool Round)
		{
			switch (Type.GetTypeCode (Value.GetType ())) {
			case TypeCode.SByte:
				return Value;
			case TypeCode.Int16:
				return Value;
			case TypeCode.Int32:
				return Value;
			case TypeCode.Int64:
				return Value;

			case TypeCode.Byte:
				return Value;
			case TypeCode.UInt16:
				return Value;
			case TypeCode.UInt32:
				return Value;
			case TypeCode.UInt64:
				return Value;

			case TypeCode.Single:
				return (Round ? (int)Math.Round ((float)Value) : (int)((float)Value));
			case TypeCode.Double:
				return (Round ? (long)Math.Round ((double)Value) : (long)((double)Value));
			case TypeCode.Decimal:
				return (Round ? Math.Round ((decimal)Value) : (decimal)Value);

			default:
				return null;
			}
		}
		#endregion
		#region UnboxToLong
		private static long UnboxToLong (object Value, bool Round)
		{
			switch (Type.GetTypeCode (Value.GetType ())) {
			case TypeCode.SByte:
				return (long)((sbyte)Value);
			case TypeCode.Int16:
				return (long)((short)Value);
			case TypeCode.Int32:
				return (long)((int)Value);
			case TypeCode.Int64:
				return (long)Value;

			case TypeCode.Byte:
				return (long)((byte)Value);
			case TypeCode.UInt16:
				return (long)((ushort)Value);
			case TypeCode.UInt32:
				return (long)((uint)Value);
			case TypeCode.UInt64:
				return (long)((ulong)Value);

			case TypeCode.Single:
				return (Round ? (long)Math.Round ((float)Value) : (long)((float)Value));
			case TypeCode.Double:
				return (Round ? (long)Math.Round ((double)Value) : (long)((double)Value));
			case TypeCode.Decimal:
				return (Round ? (long)Math.Round ((decimal)Value) : (long)((decimal)Value));

			default:
				return 0;
			}
		}
		#endregion
		#region ReplaceMetaChars
		/// <summary>
		/// Replaces the string representations of meta chars with their corresponding
		/// character values.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>A string with all string meta chars are replaced</returns>
		private static string ReplaceMetaChars (string input)
		{
			return Regex.Replace (input, @"(\\)(\d{3}|[^\d])?", new MatchEvaluator (ReplaceMetaCharsMatch));
		}
		private static string ReplaceMetaCharsMatch (Match m)
		{
			// convert octal quotes (like \040)
			if (m.Groups[2].Length == 3)
				return Convert.ToChar (Convert.ToByte (m.Groups[2].Value, 8)).ToString ();
			else {
				// convert all other special meta characters
				//TODO: \xhhh hex and possible dec !!
				switch (m.Groups[2].Value) {
				case "0":           // null
					return "\0";
				case "a":           // alert (beep)
					return "\a";
				case "b":           // BS
					return "\b";
				case "f":           // FF
					return "\f";
				case "v":           // vertical tab
					return "\v";
				case "r":           // CR
					return "\r";
				case "n":           // LF
					return "\n";
				case "t":           // Tab
					return "\t";
				default:
					throw new InvalidDataException ("Bad escape char");
				// if neither an octal quote nor a special meta character
				// so just remove the backslash
				//return m.Groups[2].Value;
				}
			}
		}
		#endregion

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
		public static int fscanf (TextReader stream, string format, out object p1)
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
		public static int scanf (string format, out object p1)
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
		public static int sscanf (string input, string format, out object p1)
		{
			List<object> results;
			int count = Parse (input, format, out results);
			p1 = results[0];
			return count;
		}
		public static int sscanf (string input, string format, out object p1, out object p2)
		{
			List<object> results;
			int count = Parse (input, format, out results);
			p1 = results[0];
			p2 = results[1];
			return count;
		}
		public static int sscanf (string input, string format, out object p1, out object p2, out object p3)
		{
			List<object> results;
			int count = Parse (input, format, out results);
			p1 = results[0];
			p2 = results[1];
			p3 = results[2];
			return count;
		}
		public static int sscanf (string input, string format, out object p1, out object p2, out object p3, out object p4)
		{
			List<object> results;
			int count = Parse (input, format, out results);
			p1 = results[0];
			p2 = results[1];
			p3 = results[2];
			p4 = results[3];
			return count;
		}
		public static int sscanf (string input, string format, out object p1, out object p2, out object p3, out object p4, out object p5)
		{
			List<object> results;
			int count = Parse (input, format, out results);
			p1 = results[0];
			p2 = results[1];
			p3 = results[2];
			p4 = results[3];
			p5 = results[4];
			return count;
		}
		#endregion

		#region Code Originally from http://www.blackbeltcoder.com/Articles/strings/a-sscanf-replacement-for-net

		// Format type specifiers
		private enum Types {
			Character,
			Decimal,
			Float,
			Hexadecimal,
			Octal,
			ScanSet,
			String,
			Unsigned
		}

		// Format modifiers
		private enum Modifiers {
			None,
			ShortShort,
			Short,
			Long,
			LongLong
		}

		// Delegate to parse a type
		private delegate bool ParseValue (TextParser input, FormatSpecifier spec, List<object> results);

		// Class to associate format type with type parser
		private class TypeParser {
			public Types Type { get; set; }
			public ParseValue Parser { get; set; }
		}

		// Class to hold format specifier information
		private class FormatSpecifier {
			public Types Type { get; set; }
			public Modifiers Modifier { get; set; }
			public int Width { get; set; }
			public bool NoResult { get; set; }
			public string ScanSet { get; set; }
			public bool ScanSetExclude { get; set; }
		}

		// Lookup table to find parser by parser type
		private static TypeParser[] _typeParsers;

		// Constructor
		static C ()
		{
			// Populate parser type lookup table
			_typeParsers = new TypeParser[] {
				new TypeParser() { Type = Types.Character, Parser = ParseCharacter },
				new TypeParser() { Type = Types.Decimal, Parser = ParseDecimal },
				new TypeParser() { Type = Types.Float, Parser = ParseFloat },
				new TypeParser() { Type = Types.Hexadecimal, Parser = ParseHexadecimal },
				new TypeParser() { Type = Types.Octal, Parser = ParseOctal },
				new TypeParser() { Type = Types.ScanSet, Parser = ParseScanSet },
				new TypeParser() { Type = Types.String, Parser = ParseString },
				new TypeParser() { Type = Types.Unsigned, Parser = ParseDecimal }
			};
		}

		/// <summary>
		/// Parses the input string according to the rules in the
		/// format string. Similar to the standard C library's
		/// sscanf() function. Parsed fields are placed in the
		/// class' Results member.
		/// </summary>
		/// <param name="input">String to parse</param>
		/// <param name="format">Specifies rules for parsing input</param>
		private static int Parse (string input, string format, out List<object> results)
		{
			TextParser inp = new TextParser (input);
			TextParser fmt = new TextParser (format);
			results = new List<object> ();
			FormatSpecifier spec = new FormatSpecifier ();
			int count = 0;

			// Process input string as indicated in format string
			while (!fmt.EndOfText && !inp.EndOfText) {
				if (ParseFormatSpecifier (fmt, spec)) {
					// Found a format specifier
					TypeParser parser = _typeParsers.First (tp => tp.Type == spec.Type);
					if (parser.Parser (inp, spec, results))
						count++;
					else
						break;
				} else if (Char.IsWhiteSpace (fmt.Peek ())) {
					// Whitespace
					inp.MovePastWhitespace ();
					fmt.MoveAhead ();
				} else if (fmt.Peek () == inp.Peek ()) {
					// Matching character
					inp.MoveAhead ();
					fmt.MoveAhead ();
				} else
					break;    // Break at mismatch
			}

			// Return number of fields successfully parsed
			return count;
		}

		/// <summary>
		/// Attempts to parse a field format specifier from the format string.
		/// </summary>
		private static bool ParseFormatSpecifier (TextParser format, FormatSpecifier spec)
		{
			// Return if not a field format specifier
			if (format.Peek () != '%')
				return false;
			format.MoveAhead ();

			// Return if "%%" (treat as '%' literal)
			if (format.Peek () == '%')
				return false;

			// Test for asterisk, which indicates result is not stored
			if (format.Peek () == '*') {
				spec.NoResult = true;
				format.MoveAhead ();
			} else
				spec.NoResult = false;

			// Parse width
			int start = format.Position;
			while (Char.IsDigit (format.Peek ()))
				format.MoveAhead ();
			if (format.Position > start)
				spec.Width = int.Parse (format.Extract (start, format.Position));
			else
				spec.Width = 0;

			// Parse modifier
			if (format.Peek () == 'h') {
				format.MoveAhead ();
				if (format.Peek () == 'h') {
					format.MoveAhead ();
					spec.Modifier = Modifiers.ShortShort;
				} else
					spec.Modifier = Modifiers.Short;
			} else if (Char.ToLower (format.Peek ()) == 'l') {
				format.MoveAhead ();
				if (format.Peek () == 'l') {
					format.MoveAhead ();
					spec.Modifier = Modifiers.LongLong;
				} else
					spec.Modifier = Modifiers.Long;
			} else
				spec.Modifier = Modifiers.None;

			// Parse type
			switch (format.Peek ()) {
			case 'c':
				spec.Type = Types.Character;
				break;
			case 'd':
			case 'i':
				spec.Type = Types.Decimal;
				break;
			case 'a':
			case 'A':
			case 'e':
			case 'E':
			case 'f':
			case 'F':
			case 'g':
			case 'G':
				spec.Type = Types.Float;
				break;
			case 'o':
				spec.Type = Types.Octal;
				break;
			case 's':
				spec.Type = Types.String;
				break;
			case 'u':
				spec.Type = Types.Unsigned;
				break;
			case 'x':
			case 'X':
				spec.Type = Types.Hexadecimal;
				break;
			case '[':
				spec.Type = Types.ScanSet;
				format.MoveAhead ();
				// Parse scan set characters
				if (format.Peek () == '^') {
					spec.ScanSetExclude = true;
					format.MoveAhead ();
				} else
					spec.ScanSetExclude = false;
				start = format.Position;
				// Treat immediate ']' as literal
				if (format.Peek () == ']')
					format.MoveAhead ();
				format.MoveTo (']');
				if (format.EndOfText)
					throw new Exception ("Type specifier expected character : ']'");
				spec.ScanSet = format.Extract (start, format.Position);
				break;
			default:
				string msg = String.Format ("Unknown format type specified : '{0}'", format.Peek ());
				throw new Exception (msg);
			}
			format.MoveAhead ();
			return true;
		}

		/// <summary>
		/// Parse a character field
		/// </summary>
		private static bool ParseCharacter (TextParser input, FormatSpecifier spec, List<object> results)
		{
			// Parse character(s)
			int start = input.Position;
			int count = (spec.Width > 1) ? spec.Width : 1;
			while (!input.EndOfText && count-- > 0)
				input.MoveAhead ();

			// Extract token
			if (count <= 0 && input.Position > start) {
				if (!spec.NoResult) {
					string token = input.Extract (start, input.Position);
					if (token.Length > 1)
						results.Add (token.ToCharArray ());
					else
						results.Add (token[0]);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parse integer field
		/// </summary>
		private static bool ParseDecimal (TextParser input, FormatSpecifier spec, List<object> results)
		{
			int radix = 10;

			// Skip any whitespace
			input.MovePastWhitespace ();

			// Parse leading sign
			int start = input.Position;
			if (input.Peek () == '+' || input.Peek () == '-') {
				input.MoveAhead ();
			} else if (input.Peek () == '0') {
				if (Char.ToLower (input.Peek (1)) == 'x') {
					radix = 16;
					input.MoveAhead (2);
				} else {
					radix = 8;
					input.MoveAhead ();
				}
			}

			// Parse digits
			while (IsValidDigit (input.Peek (), radix))
				input.MoveAhead ();

			// Don't exceed field width
			if (spec.Width > 0) {
				int count = input.Position - start;
				if (spec.Width < count)
					input.MoveAhead (spec.Width - count);
			}

			// Extract token
			if (input.Position > start) {
				if (!spec.NoResult) {
					if (spec.Type == Types.Decimal)
						AddSigned (input.Extract (start, input.Position), spec.Modifier, radix, results);
					else
						AddUnsigned (input.Extract (start, input.Position), spec.Modifier, radix, results);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parse a floating-point field
		/// </summary>
		private static bool ParseFloat (TextParser input, FormatSpecifier spec, List<object> results)
		{
			// Skip any whitespace
			input.MovePastWhitespace ();

			// Parse leading sign
			int start = input.Position;
			if (input.Peek () == '+' || input.Peek () == '-')
				input.MoveAhead ();

			// Parse digits
			bool hasPoint = false;
			while (Char.IsDigit (input.Peek ()) || input.Peek () == '.') {
				if (input.Peek () == '.') {
					if (hasPoint)
						break;
					hasPoint = true;
				}
				input.MoveAhead ();
			}

			// Parse exponential notation
			if (Char.ToLower (input.Peek ()) == 'e') {
				input.MoveAhead ();
				if (input.Peek () == '+' || input.Peek () == '-')
					input.MoveAhead ();
				while (Char.IsDigit (input.Peek ()))
					input.MoveAhead ();
			}

			// Don't exceed field width
			if (spec.Width > 0) {
				int count = input.Position - start;
				if (spec.Width < count)
					input.MoveAhead (spec.Width - count);
			}

			// Because we parse the exponential notation before we apply
			// any field-width constraint, it becomes awkward to verify
			// we have a valid floating point token. To prevent an
			// exception, we use TryParse() here instead of Parse().
			double result;

			// Extract token
			if (input.Position > start &&
				double.TryParse (input.Extract (start, input.Position), out result)) {
				if (!spec.NoResult) {
					if (spec.Modifier == Modifiers.Long ||
						spec.Modifier == Modifiers.LongLong)
						results.Add (result);
					else
						results.Add ((float)result);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parse hexadecimal field
		/// </summary>
		private static bool ParseHexadecimal (TextParser input, FormatSpecifier spec, List<object> results)
		{
			// Skip any whitespace
			input.MovePastWhitespace ();

			// Parse 0x prefix
			int start = input.Position;
			if (input.Peek () == '0' && input.Peek (1) == 'x')
				input.MoveAhead (2);

			// Parse digits
			while (IsValidDigit (input.Peek (), 16))
				input.MoveAhead ();

			// Don't exceed field width
			if (spec.Width > 0) {
				int count = input.Position - start;
				if (spec.Width < count)
					input.MoveAhead (spec.Width - count);
			}

			// Extract token
			if (input.Position > start) {
				if (!spec.NoResult)
					AddUnsigned (input.Extract (start, input.Position), spec.Modifier, 16, results);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parse an octal field
		/// </summary>
		private static bool ParseOctal (TextParser input, FormatSpecifier spec, List<object> results)
		{
			// Skip any whitespace
			input.MovePastWhitespace ();

			// Parse digits
			int start = input.Position;
			while (IsValidDigit (input.Peek (), 8))
				input.MoveAhead ();

			// Don't exceed field width
			if (spec.Width > 0) {
				int count = input.Position - start;
				if (spec.Width < count)
					input.MoveAhead (spec.Width - count);
			}

			// Extract token
			if (input.Position > start) {
				if (!spec.NoResult)
					AddUnsigned (input.Extract (start, input.Position), spec.Modifier, 8, results);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parse a scan-set field
		/// </summary>
		private static bool ParseScanSet (TextParser input, FormatSpecifier spec, List<object> results)
		{
			// Parse characters
			int start = input.Position;
			if (!spec.ScanSetExclude) {
				while (spec.ScanSet.Contains (input.Peek ()))
					input.MoveAhead ();
			} else {
				while (!input.EndOfText && !spec.ScanSet.Contains (input.Peek ()))
					input.MoveAhead ();
			}

			// Don't exceed field width
			if (spec.Width > 0) {
				int count = input.Position - start;
				if (spec.Width < count)
					input.MoveAhead (spec.Width - count);
			}

			// Extract token
			if (input.Position > start) {
				if (!spec.NoResult)
					results.Add (input.Extract (start, input.Position));
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parse a string field
		/// </summary>
		private static bool ParseString (TextParser input, FormatSpecifier spec, List<object> results)
		{
			// Skip any whitespace
			input.MovePastWhitespace ();

			// Parse string characters
			int start = input.Position;
			while (!input.EndOfText && !Char.IsWhiteSpace (input.Peek ()))
				input.MoveAhead ();

			// Don't exceed field width
			if (spec.Width > 0) {
				int count = input.Position - start;
				if (spec.Width < count)
					input.MoveAhead (spec.Width - count);
			}

			// Extract token
			if (input.Position > start) {
				if (!spec.NoResult)
					results.Add (input.Extract (start, input.Position));
				return true;
			}
			return false;
		}

		// Determines if the given digit is valid for the given radix
		private static bool IsValidDigit (char c, int radix)
		{
			int i = "0123456789abcdef".IndexOf (Char.ToLower (c));
			if (i >= 0 && i < radix)
				return true;
			return false;
		}

		// Parse signed token and add to results
		private static void AddSigned (string token, Modifiers mod, int radix, List<object> results)
		{
			object obj;
			if (mod == Modifiers.ShortShort)
				obj = Convert.ToSByte (token, radix);
			else if (mod == Modifiers.Short)
				obj = Convert.ToInt16 (token, radix);
			else if (mod == Modifiers.Long ||
				mod == Modifiers.LongLong)
				obj = Convert.ToInt64 (token, radix);
			else
				obj = Convert.ToInt32 (token, radix);
			results.Add (obj);
		}

		// Parse unsigned token and add to results
		private static void AddUnsigned (string token, Modifiers mod, int radix, List<object> results)
		{
			object obj;
			if (mod == Modifiers.ShortShort)
				obj = Convert.ToByte (token, radix);
			else if (mod == Modifiers.Short)
				obj = Convert.ToUInt16 (token, radix);
			else if (mod == Modifiers.Long ||
				mod == Modifiers.LongLong)
				obj = Convert.ToUInt64 (token, radix);
			else
				obj = Convert.ToUInt32 (token, radix);
			results.Add (obj);
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
			StringBuilder sb = new StringBuilder ();
			#region Variables
			//                         +
			Regex r = new Regex (@"\%(\d*\$)?([\'\#\-\+ ]*)(\d*)(?:\.(\d+))?([hl])?([dioxXucsfeEgGpn%])");
			//"%[parameter][flags][width][.precision][length]type"
			Match m = null;
			string w = String.Empty;
			int defaultParamIx = 0;
			int paramIx;
			object o = null;

			bool flagLeft2Right = false;
			bool flagAlternate = false;
			bool flagPositiveSign = false;
			bool flagPositiveSpace = false;
			bool flagZeroPadding = false;
			bool flagGroupThousands = false;

			int fieldLength = 0;
			int fieldPrecision = 0;
			char shortLongIndicator = '\0';
			char formatSpecifier = '\0';
			char paddingCharacter = ' ';
			#endregion

			// find all format parameters in format string
			sb.Append (format);
			m = r.Match (sb.ToString ());
			while (m.Success) {
				#region parameter index
				paramIx = defaultParamIx;
				if (m.Groups[1] != null && m.Groups[1].Value.Length > 0) {
					string val = m.Groups[1].Value.Substring (0, m.Groups[1].Value.Length - 1);
					paramIx = Convert.ToInt32 (val) - 1;
				};
				#endregion

				#region format flags
				// extract format flags
				flagAlternate = false;
				flagLeft2Right = false;
				flagPositiveSign = false;
				flagPositiveSpace = false;
				flagZeroPadding = false;
				flagGroupThousands = false;
				if (m.Groups[2] != null && m.Groups[2].Value.Length > 0) {
					string flags = m.Groups[2].Value;

					flagAlternate = (flags.IndexOf ('#') >= 0);
					flagLeft2Right = (flags.IndexOf ('-') >= 0);
					flagPositiveSign = (flags.IndexOf ('+') >= 0);
					flagPositiveSpace = (flags.IndexOf (' ') >= 0);
					flagGroupThousands = (flags.IndexOf ('\'') >= 0);

					// positive + indicator overrides a
					// positive space character
					if (flagPositiveSign && flagPositiveSpace)
						flagPositiveSpace = false;
				}
				#endregion

				#region field length
				// extract field length and 
				// pading character
				paddingCharacter = ' ';
				fieldLength = int.MinValue;
				if (m.Groups[3] != null && m.Groups[3].Value.Length > 0) {
					fieldLength = Convert.ToInt32 (m.Groups[3].Value);
					flagZeroPadding = (m.Groups[3].Value[0] == '0');
				}
				#endregion

				if (flagZeroPadding)
					paddingCharacter = '0';

				// left2right allignment overrides zero padding
				if (flagLeft2Right && flagZeroPadding) {
					flagZeroPadding = false;
					paddingCharacter = ' ';
				}

				#region field precision
				// extract field precision
				fieldPrecision = int.MinValue;
				if (m.Groups[4] != null && m.Groups[4].Value.Length > 0)
					fieldPrecision = Convert.ToInt32 (m.Groups[4].Value);
				#endregion

				#region short / long indicator
				// extract short / long indicator
				shortLongIndicator = Char.MinValue;
				if (m.Groups[5] != null && m.Groups[5].Value.Length > 0)
					shortLongIndicator = m.Groups[5].Value[0];
				#endregion

				#region format specifier
				// extract format
				formatSpecifier = Char.MinValue;
				if (m.Groups[6] != null && m.Groups[6].Value.Length > 0)
					formatSpecifier = m.Groups[6].Value[0];
				#endregion

				// default precision is 6 digits if none is specified except
				if (fieldPrecision == int.MinValue &&
					formatSpecifier != 's' &&
					formatSpecifier != 'c' &&
					Char.ToUpper (formatSpecifier) != 'X' &&
					formatSpecifier != 'o')
					fieldPrecision = 6;

				#region get next value parameter
				// get next value parameter and convert value parameter depending on short / long indicator
				if (parameters == null || paramIx >= parameters.Length)
					o = null;
				else {
					o = parameters[paramIx];

					if (shortLongIndicator == 'h') {
						if (o is int)
							o = (short)((int)o);
						else if (o is long)
							o = (short)((long)o);
						else if (o is uint)
							o = (ushort)((uint)o);
						else if (o is ulong)
							o = (ushort)((ulong)o);
					} else if (shortLongIndicator == 'l') {
						if (o is short)
							o = (long)((short)o);
						else if (o is int)
							o = (long)((int)o);
						else if (o is ushort)
							o = (ulong)((ushort)o);
						else if (o is uint)
							o = (ulong)((uint)o);
					}
				}
				#endregion

				// convert value parameters to a string depending on the formatSpecifier
				w = String.Empty;
				switch (formatSpecifier) {
				#region % - character
				case '%':   // % character
					w = "%";
					break;
				#endregion
				#region d - integer
				case 'd':   // integer
					w = FormatNumber ((flagGroupThousands ? "n" : "d"), flagAlternate,
									fieldLength, int.MinValue, flagLeft2Right,
									flagPositiveSign, flagPositiveSpace,
									paddingCharacter, o);
					defaultParamIx++;
					break;
				#endregion
				#region i - integer
				case 'i':   // integer
					goto case 'd';
				#endregion
				#region o - octal integer
				case 'o':   // octal integer - no leading zero
					w = FormatOct ("o", flagAlternate,
									fieldLength, int.MinValue, flagLeft2Right,
									paddingCharacter, o);
					defaultParamIx++;
					break;
				#endregion
				#region x - hex integer
				case 'x':   // hex integer - no leading zero
					w = FormatHex ("x", flagAlternate,
									fieldLength, fieldPrecision, flagLeft2Right,
									paddingCharacter, o);
					defaultParamIx++;
					break;
				#endregion
				#region X - hex integer
				case 'X':   // same as x but with capital hex characters
					w = FormatHex ("X", flagAlternate,
									fieldLength, fieldPrecision, flagLeft2Right,
									paddingCharacter, o);
					defaultParamIx++;
					break;
				#endregion
				#region u - unsigned integer
				case 'u':   // unsigned integer
					w = FormatNumber ((flagGroupThousands ? "n" : "d"), flagAlternate,
									fieldLength, int.MinValue, flagLeft2Right,
									false, false,
									paddingCharacter, ToUnsigned (o));
					defaultParamIx++;
					break;
				#endregion
				#region c - character
				case 'c':   // character
					if (IsNumericType (o))
						w = Convert.ToChar (o).ToString ();
					else if (o is char)
						w = ((char)o).ToString ();
					else if (o is string && ((string)o).Length > 0)
						w = ((string)o)[0].ToString ();
					defaultParamIx++;
					break;
				#endregion
				#region s - string
				case 's':   // string
					string t = "{0" + (fieldLength != int.MinValue ? "," + (flagLeft2Right ? "-" : String.Empty) + fieldLength.ToString () : String.Empty) + ":s}";
					w = o.ToString ();
					if (fieldPrecision >= 0)
						w = w.Substring (0, fieldPrecision);

					if (fieldLength != int.MinValue)
						if (flagLeft2Right)
							w = w.PadRight (fieldLength, paddingCharacter);
						else
							w = w.PadLeft (fieldLength, paddingCharacter);
					defaultParamIx++;
					break;
				#endregion
				#region f - double number
				case 'f':   // double
					w = FormatNumber ((flagGroupThousands ? "n" : "f"), flagAlternate,
									fieldLength, fieldPrecision, flagLeft2Right,
									flagPositiveSign, flagPositiveSpace,
									paddingCharacter, o);
					defaultParamIx++;
					break;
				#endregion
				#region e - exponent number
				case 'e':   // double / exponent
					w = FormatNumber ("e", flagAlternate,
									fieldLength, fieldPrecision, flagLeft2Right,
									flagPositiveSign, flagPositiveSpace,
									paddingCharacter, o);
					defaultParamIx++;
					break;
				#endregion
				#region E - exponent number
				case 'E':   // double / exponent
					w = FormatNumber ("E", flagAlternate,
									fieldLength, fieldPrecision, flagLeft2Right,
									flagPositiveSign, flagPositiveSpace,
									paddingCharacter, o);
					defaultParamIx++;
					break;
				#endregion
				#region g - general number
				case 'g':   // double / exponent
					w = FormatNumber ("g", flagAlternate,
									fieldLength, fieldPrecision, flagLeft2Right,
									flagPositiveSign, flagPositiveSpace,
									paddingCharacter, o);
					defaultParamIx++;
					break;
				#endregion
				#region G - general number
				case 'G':   // double / exponent
					w = FormatNumber ("G", flagAlternate,
									fieldLength, fieldPrecision, flagLeft2Right,
									flagPositiveSign, flagPositiveSpace,
									paddingCharacter, o);
					defaultParamIx++;
					break;
				#endregion
				#region p - pointer
				case 'p':   // pointer
					if (o is IntPtr)
						w = "0x" + ((IntPtr)o).ToString ("x");
					defaultParamIx++;
					break;
				#endregion
				#region n - number of processed chars so far
				case 'n':   // number of characters so far
					w = FormatNumber ("d", flagAlternate,
									fieldLength, int.MinValue, flagLeft2Right,
									flagPositiveSign, flagPositiveSpace,
									paddingCharacter, m.Index);
					break;
				#endregion
				default:
					w = String.Empty;
					defaultParamIx++;
					break;
				}

				// replace format parameter with parameter value
				// and start searching for the next format parameter
				// AFTER the position of the current inserted value
				// to prohibit recursive matches if the value also
				// includes a format specifier
				sb.Remove (m.Index, m.Length);
				sb.Insert (m.Index, w);
				m = r.Match (sb.ToString (), m.Index + w.Length);
			}
			result = sb.ToString ();
			return result.Length;
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
			if (string.IsNullOrEmpty (mode))
				throw new Exception ("invalid mode");
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
				throw new Exception ("invalid mode (2)");
			}
			FileStream stream = new FileStream (filename, m, a, FileShare.Read);
			FILE file = new FILE (stream);
			return file;
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

		#region Private Methods
		#region FormatOCT
		private static string FormatOct (string NativeFormat, bool Alternate,
											int FieldLength, int FieldPrecision,
											bool Left2Right,
											char Padding, object Value)
		{
			string w = String.Empty;
			string lengthFormat = "{0" + (FieldLength != int.MinValue ?
											"," + (Left2Right ?
													"-" :
													String.Empty) + FieldLength.ToString () :
											String.Empty) + "}";

			if (IsNumericType (Value)) {
				w = Convert.ToString (UnboxToLong (Value, true), 8);

				if (Left2Right || Padding == ' ') {
					if (Alternate && w != "0")
						w = "0" + w;
					w = String.Format (lengthFormat, w);
				} else {
					if (FieldLength != int.MinValue)
						w = w.PadLeft (FieldLength - (Alternate && w != "0" ? 1 : 0), Padding);
					if (Alternate && w != "0")
						w = "0" + w;
				}
			}

			return w;
		}
		#endregion
		#region FormatHEX
		private static string FormatHex (string NativeFormat, bool Alternate,
											int FieldLength, int FieldPrecision,
											bool Left2Right,
											char Padding, object Value)
		{
			string w = String.Empty;
			string lengthFormat = "{0" + (FieldLength != int.MinValue ?
											"," + (Left2Right ?
													"-" :
													String.Empty) + FieldLength.ToString () :
											String.Empty) + "}";
			string numberFormat = "{0:" + NativeFormat + (FieldPrecision != int.MinValue ?
											FieldPrecision.ToString () :
											String.Empty) + "}";

			if (IsNumericType (Value)) {
				w = String.Format (numberFormat, Value);

				if (Left2Right || Padding == ' ') {
					if (Alternate)
						w = (NativeFormat == "x" ? "0x" : "0X") + w;
					w = String.Format (lengthFormat, w);
				} else {
					if (FieldLength != int.MinValue)
						w = w.PadLeft (FieldLength - (Alternate ? 2 : 0), Padding);
					if (Alternate)
						w = (NativeFormat == "x" ? "0x" : "0X") + w;
				}
			}

			return w;
		}
		#endregion
		#region FormatNumber
		private static string FormatNumber (string NativeFormat, bool Alternate,
											int FieldLength, int FieldPrecision,
											bool Left2Right,
											bool PositiveSign, bool PositiveSpace,
											char Padding, object Value)
		{
			string w = String.Empty;
			string lengthFormat = "{0" + (FieldLength != int.MinValue ?
											"," + (Left2Right ?
													"-" :
													String.Empty) + FieldLength.ToString () :
											String.Empty) + "}";
			string numberFormat = "{0:" + NativeFormat + (FieldPrecision != int.MinValue ?
											FieldPrecision.ToString () :
											"0") + "}";

			if (IsNumericType (Value)) {
				w = String.Format (numberFormat, Value);

				if (Left2Right || Padding == ' ') {
					if (IsPositive (Value, true))
						w = (PositiveSign ?
								"+" : (PositiveSpace ? " " : String.Empty)) + w;
					w = String.Format (lengthFormat, w);
				} else {
					if (w.StartsWith ("-"))
						w = w.Substring (1);
					if (FieldLength != int.MinValue)
						w = w.PadLeft (FieldLength - 1, Padding);
					if (IsPositive (Value, true))
						w = (PositiveSign ?
								"+" : (PositiveSpace ?
										" " : (FieldLength != int.MinValue ?
												Padding.ToString () : String.Empty))) + w;
					else
						w = "-" + w;
				}
			}

			return w;
		}
		#endregion
		#endregion

		//typedef long    fpos_t;

		//#define _IOFBF  0
		//#define _IOLBF  1
		//#define _IONBF  2

		//#define FOPEN_MAX 8
		//#define FILENAME_MAX 100
		//#define BUFSIZ  256
		//#define L_tmpnam    12
		//#define SEEK_CUR    1
		//#define SEEK_END    2
		//#define SEEK_SET    0
		//#define TMP_MAX     25
		//extern  FILE    _streams[];

		//void clearerr(FILE *);
		//int  feof(FILE *);
		//int  ferror(FILE *);
		//int  fflush(FILE *);
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
			return errno.ToString ();
		}


		//int  printf(const char *, ...);
		//int  remove(const char *);
		//int  rename(const char *,const char *);
		//void rewind(FILE *);
		//int  scanf(const char *, ...);
		//void setbuf(FILE *, char *);
		//int  setvbuf(FILE *, char *, int, size_t );
		//int  sprintf(char *, const char *, ...);
		//int  sscanf(const char *, const char *, ...);
		//FILE *tmpfile(void);
		//char *tmpnam(char *);

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

	}
}
