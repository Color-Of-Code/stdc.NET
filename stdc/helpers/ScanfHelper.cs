﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stdc {
	internal static class ScanfHelper {

		// Code Originally from http://www.blackbeltcoder.com/Articles/strings/a-sscanf-replacement-for-net

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
		static ScanfHelper ()
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
		public static int Parse (string input, string format, out List<object> results)
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

	}
}
