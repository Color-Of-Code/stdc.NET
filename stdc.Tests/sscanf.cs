namespace stdc.Tests;

using System;
using NUnit.Framework;

[TestFixture]
public class scanf
{
    private string sepDecimal = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
    private string sep1000 = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;

    #region Setup Tests
    [SetUp]
    public void SetUp()
    {
    }
    #endregion

    #region Tests
    #region PositiveInteger
    [Category("Integer")]
    [Test(Description = "Test positive signed integer format %d / %i")]
    public void PositiveInteger()
    {
        Console.WriteLine("Test positive signed integer format %d / %i");
        Console.WriteLine("--------------------------------------------------------------------------------");

        int result42 = 42;
        Assert.That(RunTest("[%d]", "[42]", result42));
        Assert.That(RunTest("[%i]", "[42]", result42));
        //Assert.IsTrue (RunTest ("[%ld]", "[42]", result42));
        Console.WriteLine("\n\n");
    }
    #endregion
    #region NegativeInteger
    [Category("Integer")]
    [Test(Description = "Test negative signed integer format %d / %i")]
    public void NegativeInteger()
    {
        Console.WriteLine("Test negative signed integer format %d / %i");
        Console.WriteLine("--------------------------------------------------------------------------------");

        int result42 = -42;
        Assert.That(RunTest("[%d]", "[-42]", result42));
        Assert.That(RunTest("[%i]", "[-42]", result42));
        //Assert.IsTrue (RunTest ("[%ld]", "[-42]", result42));

        Console.WriteLine("\n\n");
    }
    #endregion
    //#region UnsignedInteger
    //[Category ("Integer")]
    //[Test (Description = "Test unsigned integer format %u")]
    //public void UnsignedInteger ()
    //{
    //    Console.WriteLine ("Test unsigned integer format %u");
    //    Console.WriteLine ("--------------------------------------------------------------------------------");

    //    Assert.IsTrue (RunTest ("[%u]", "[42]", 42));
    //    Assert.IsTrue (RunTest ("[%u]", "[4294967254]", -42));
    //    Assert.IsTrue (RunTest ("[%u]", "[65537]", 65537));

    //    Console.WriteLine ("\n\n");
    //}
    //#endregion
    //#region Float
    //[Category ("Float")]
    //[Test (Description = "Test float format %f")]
    //public void Floats ()
    //{
    //    Console.WriteLine ("Test float format %f");
    //    Console.WriteLine ("--------------------------------------------------------------------------------");

    //    Assert.IsTrue (RunTest ("[%f]", String.Format ("[42{0}000000]", sepDecimal), 42));
    //    Assert.IsTrue (RunTest ("[%f]", String.Format ("[42{0}500000]", sepDecimal), 42.5));

    //    Assert.IsTrue (RunTest ("[%+f]", String.Format ("[+42{0}000000]", sepDecimal), 42));
    //    Assert.IsTrue (RunTest ("[%+f]", String.Format ("[+42{0}500000]", sepDecimal), 42.5));

    //    Assert.IsTrue (RunTest ("[%f]", String.Format ("[-42{0}000000]", sepDecimal), -42));
    //    Assert.IsTrue (RunTest ("[%f]", String.Format ("[-42{0}500000]", sepDecimal), -42.5));

    //    Assert.IsTrue (RunTest ("[%+f]", String.Format ("[-42{0}000000]", sepDecimal), -42));
    //    Assert.IsTrue (RunTest ("[%+f]", String.Format ("[-42{0}500000]", sepDecimal), -42.5));

    //    // -----

    //    Assert.IsTrue (RunTest ("[%.2f]", String.Format ("[42{0}00]", sepDecimal), 42));
    //    Assert.IsTrue (RunTest ("[%.2f]", String.Format ("[42{0}50]", sepDecimal), 42.5));

    //    Assert.IsTrue (RunTest ("[%+.2f]", String.Format ("[+42{0}00]", sepDecimal), 42));
    //    Assert.IsTrue (RunTest ("[%+.2f]", String.Format ("[+42{0}50]", sepDecimal), 42.5));

    //    Assert.IsTrue (RunTest ("[%.2f]", String.Format ("[-42{0}00]", sepDecimal), -42));
    //    Assert.IsTrue (RunTest ("[%.2f]", String.Format ("[-42{0}50]", sepDecimal), -42.5));

    //    Assert.IsTrue (RunTest ("[%+.2f]", String.Format ("[-42{0}00]", sepDecimal), -42));
    //    Assert.IsTrue (RunTest ("[%+.2f]", String.Format ("[-42{0}50]", sepDecimal), -42.5));

    //    Console.WriteLine ("\n\n");
    //}
    //#endregion
    //#region Exponent
    //[Category ("Exponent")]
    //[Test (Description = "Test exponent format %f")]
    //public void Exponents ()
    //{
    //    Console.WriteLine ("Test exponent format %f");
    //    Console.WriteLine ("--------------------------------------------------------------------------------");

    //    Assert.IsTrue (RunTest ("[%e]", String.Format ("[4{0}200000e+001]", sepDecimal), 42));
    //    Assert.IsTrue (RunTest ("[%e]", String.Format ("[4{0}250000e+001]", sepDecimal), 42.5));

    //    Assert.IsTrue (RunTest ("[%+E]", String.Format ("[+4{0}200000E+001]", sepDecimal), 42));
    //    Assert.IsTrue (RunTest ("[%+E]", String.Format ("[+4{0}250000E+001]", sepDecimal), 42.5));

    //    Assert.IsTrue (RunTest ("[%e]", String.Format ("[-4{0}200000e+001]", sepDecimal), -42));
    //    Assert.IsTrue (RunTest ("[%e]", String.Format ("[-4{0}250000e+001]", sepDecimal), -42.5));

    //    Assert.IsTrue (RunTest ("[%+e]", String.Format ("[-4{0}200000e+001]", sepDecimal), -42));
    //    Assert.IsTrue (RunTest ("[%+e]", String.Format ("[-4{0}250000e+001]", sepDecimal), -42.5));

    //    // -----

    //    Assert.IsTrue (RunTest ("[%.2e]", String.Format ("[4{0}20e+001]", sepDecimal), 42));
    //    Assert.IsTrue (RunTest ("[%.2e]", String.Format ("[4{0}25e+001]", sepDecimal), 42.5));

    //    Assert.IsTrue (RunTest ("[%+.2E]", String.Format ("[+4{0}20E+001]", sepDecimal), 42));
    //    Assert.IsTrue (RunTest ("[%+.2E]", String.Format ("[+4{0}25E+001]", sepDecimal), 42.5));

    //    Assert.IsTrue (RunTest ("[%.2e]", String.Format ("[-4{0}20e+001]", sepDecimal), -42));
    //    Assert.IsTrue (RunTest ("[%.2e]", String.Format ("[-4{0}25e+001]", sepDecimal), -42.5));

    //    Assert.IsTrue (RunTest ("[%+.2e]", String.Format ("[-4{0}20e+001]", sepDecimal), -42));
    //    Assert.IsTrue (RunTest ("[%+.2e]", String.Format ("[-4{0}25e+001]", sepDecimal), -42.5));

    //    Console.WriteLine ("\n\n");
    //}
    //#endregion
    #region Character
    [Category("Character")]
    [Test(Description = "Character format %c")]
    public void CharacterFormat()
    {
        Console.WriteLine("Test character formats %c");
        Console.WriteLine("--------------------------------------------------------------------------------");

        char resultA = 'A';
        Assert.That(RunTest("[%c]", "[A]", resultA));

        Console.WriteLine("\n\n");
    }
    #endregion

    #region Strings
    [Category("String")]
    [Test(Description = "Test string format %s")]
    public void Strings()
    {
        Console.WriteLine("Test string format %s");
        Console.WriteLine("--------------------------------------------------------------------------------");

        //string result1 = "This is a test";
        string result1 = "This";
        Assert.That(RunTest("[%s]", "[This is a test]", result1));

        //string result2 = "A test with %";
        string result2 = "A";
        Assert.That(RunTest("[%s]", "[A test with %]", result2));

        //string result3 = "A test with %s inside";
        string result3 = "A";
        Assert.That(RunTest("[%s]", "[A test with %s inside]", result3));

        //Assert.IsTrue (RunTest ("[%% %s %%]", "[% % Another test % %]", "% Another test %"));

        Console.WriteLine("\n\n");
    }
    #endregion
    //#region Hex
    //[Category ("HEX")]
    //[Test (Description = "Test hex format %x / %X")]
    //public void Hex ()
    //{
    //    Console.WriteLine ("Test hex format %x / %X");
    //    Console.WriteLine ("--------------------------------------------------------------------------------");

    //    Assert.IsTrue (RunTest ("[%x]", "[2a]", 42));
    //    Assert.IsTrue (RunTest ("[%X]", "[2A]", 42));
    //    Assert.IsTrue (RunTest ("[%5x]", "[   2a]", 42));
    //    Assert.IsTrue (RunTest ("[%5X]", "[   2A]", 42));
    //    Assert.IsTrue (RunTest ("[%05x]", "[0002a]", 42));
    //    Assert.IsTrue (RunTest ("[%05X]", "[0002A]", 42));
    //    Assert.IsTrue (RunTest ("[%-05x]", "[2a   ]", 42));
    //    Assert.IsTrue (RunTest ("[%-05X]", "[2A   ]", 42));

    //    Assert.IsTrue (RunTest ("[%#x]", "[0x2a]", 42));
    //    Assert.IsTrue (RunTest ("[%#X]", "[0X2A]", 42));
    //    Assert.IsTrue (RunTest ("[%#5x]", "[ 0x2a]", 42));
    //    Assert.IsTrue (RunTest ("[%#5X]", "[ 0X2A]", 42));
    //    Assert.IsTrue (RunTest ("[%#05x]", "[0x02a]", 42));
    //    Assert.IsTrue (RunTest ("[%#05X]", "[0X02A]", 42));
    //    Assert.IsTrue (RunTest ("[%#-05x]", "[0x2a ]", 42));
    //    Assert.IsTrue (RunTest ("[%#-05X]", "[0X2A ]", 42));

    //    Assert.IsTrue (RunTest ("[%.2x]", "[05]", 5));

    //    Console.WriteLine ("\n\n");
    //}
    //#endregion
    //#region Octal
    //[Category ("Octal")]
    //[Test (Description = "Test octal format %o")]
    //public void Octal ()
    //{
    //    Console.WriteLine ("Test octal format %o");
    //    Console.WriteLine ("--------------------------------------------------------------------------------");

    //    Assert.IsTrue (RunTest ("[%o]", "[52]", 42));
    //    Assert.IsTrue (RunTest ("[%o]", "[52]", 42));
    //    Assert.IsTrue (RunTest ("[%5o]", "[   52]", 42));
    //    Assert.IsTrue (RunTest ("[%5o]", "[   52]", 42));
    //    Assert.IsTrue (RunTest ("[%05o]", "[00052]", 42));
    //    Assert.IsTrue (RunTest ("[%05o]", "[00052]", 42));
    //    Assert.IsTrue (RunTest ("[%-05o]", "[52   ]", 42));
    //    Assert.IsTrue (RunTest ("[%-05o]", "[52   ]", 42));

    //    Assert.IsTrue (RunTest ("[%#o]", "[052]", 42));
    //    Assert.IsTrue (RunTest ("[%#o]", "[052]", 42));
    //    Assert.IsTrue (RunTest ("[%#5o]", "[  052]", 42));
    //    Assert.IsTrue (RunTest ("[%#5o]", "[  052]", 42));
    //    Assert.IsTrue (RunTest ("[%#05o]", "[00052]", 42));
    //    Assert.IsTrue (RunTest ("[%#05o]", "[00052]", 42));
    //    Assert.IsTrue (RunTest ("[%#-05o]", "[052  ]", 42));
    //    Assert.IsTrue (RunTest ("[%#-05o]", "[052  ]", 42));

    //    Console.WriteLine ("\n\n");
    //}
    //#endregion
    #endregion

    #region Destroy Tests
    [TearDown]
    public void TearDown()
    {
    }
    #endregion

    #region Private Methods
    #region RunTest
    [Ignore("")]
    private bool RunTest<T>(string format, string input, T expected)
    {
        object o;
        C.sscanf(input, format, out o);
        T result = (T)o;
        Console.WriteLine("Format:\t{0,-30}\tInput:\t{1}\nResult:\t{2}",
            format, input, result);
        if (result == null || result.Equals(expected))
        {
            Console.WriteLine();
            return true;
        }
        else
        {
            Console.WriteLine("*** ERROR ***\n");
            return false;
        }
    }
    #endregion
    #endregion
}
