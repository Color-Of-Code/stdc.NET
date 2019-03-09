// Gardens Point Scanner Generator
// Copyright (c) K John Gough, QUT 2006-2014
// (see accompanying GPLEXcopyright.rtf)


using System;
using System.Collections;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace QUT.Gplex
{
    internal static class CharCategory
    {
        static BitArray idStart = new BitArray(32, false);
        static BitArray idPart = new BitArray(32, false);

        /// <summary>
        /// This method builds bit-sets to represent the UnicodeCategory
        /// values that belong to the IdStart and IdContinue predicate.
        /// This uses the values as defined for C# V3, with the exception
        /// that format characters (General Category Cf) are not included
        /// in the IdPart set.  This is so that those identifiers that 
        /// include a Cf character can have a different accept state in
        /// the automaton, and only these identifer need be canonicalized.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        // Reason for message suppression: BitArray initializer for
        // enumeration types is not understandable by human readers.
        static CharCategory()
        {
            idStart[(int)UnicodeCategory.UppercaseLetter] = true;
            idStart[(int)UnicodeCategory.LowercaseLetter] = true;
            idStart[(int)UnicodeCategory.TitlecaseLetter] = true;
            idStart[(int)UnicodeCategory.ModifierLetter] = true;
            idStart[(int)UnicodeCategory.OtherLetter] = true;
            idStart[(int)UnicodeCategory.LetterNumber] = true;

            idPart[(int)UnicodeCategory.UppercaseLetter] = true;
            idPart[(int)UnicodeCategory.LowercaseLetter] = true;
            idPart[(int)UnicodeCategory.TitlecaseLetter] = true;
            idPart[(int)UnicodeCategory.ModifierLetter] = true;
            idPart[(int)UnicodeCategory.OtherLetter] = true;
            idPart[(int)UnicodeCategory.LetterNumber] = true;

            idPart[(int)UnicodeCategory.DecimalDigitNumber] = true;

            idPart[(int)UnicodeCategory.ConnectorPunctuation] = true;

            idPart[(int)UnicodeCategory.NonSpacingMark] = true;
            idPart[(int)UnicodeCategory.SpacingCombiningMark] = true;

            // Format characters are not included, so that we
            // may use a different semantic action with identifiers
            // that require canonicalization.
            //
            // idPart[(int)UnicodeCategory.Format] = true;
        }

        //
        //  These predicates are only used at compile time, when building
        //  the multilevel structure that performs the tests at runtime.
        //
        internal static bool IsIdStart(char chr)
        {
            if (chr == '_')
                return true;
            UnicodeCategory theCat = Char.GetUnicodeCategory(chr);
            return idStart[(int)theCat];
        }

        internal static bool IsIdStart(string str, int index)
        {
            if (str[index] == '_')
                return true;
            UnicodeCategory theCat = Char.GetUnicodeCategory(str, index);
            return idStart[(int)theCat];
        }

        internal static bool IsIdPart(char chr)
        {
            UnicodeCategory theCat = Char.GetUnicodeCategory(chr);
            return idPart[(int)theCat];
        }

        internal static bool IsIdPart(string str, int index)
        {
            UnicodeCategory theCat = Char.GetUnicodeCategory(str, index);
            return idPart[(int)theCat];
        }

        internal static bool IsFormat(char chr)
        {
            UnicodeCategory theCat = Char.GetUnicodeCategory(chr);
            return theCat == UnicodeCategory.Format;
        }

        internal static bool IsFormat(string str, int index)
        {
            UnicodeCategory theCat = Char.GetUnicodeCategory(str, index);
            return theCat == UnicodeCategory.Format;
        }
    }
}
