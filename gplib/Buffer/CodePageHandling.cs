using System;
using System.Globalization;
using System.Text;

namespace QUT.Gplib
{
    // ==============================================================
    // ============      class CodePageHandling         =============
    // ==============================================================
    // #if (!NOFILES)
    public static class CodePageHandling
    {
        public static int GetCodePage(string option)
        {
            string command = option.ToUpperInvariant();
            if (command.StartsWith("CodePage:", StringComparison.OrdinalIgnoreCase))
                command = command.Substring(9);
            try
            {
                if (command.Equals("RAW"))
                    return -1;
                else if (command.Equals("GUESS"))
                    return -2;
                else if (command.Equals("DEFAULT"))
                    return 0;
                else if (char.IsDigit(command[0]))
                    return int.Parse(command, CultureInfo.InvariantCulture);
                else
                {
                    Encoding enc = Encoding.GetEncoding(command);
                    return enc.CodePage;
                }
            }
            catch (FormatException)
            {
                Console.Error.WriteLine(
                    "Invalid format \"{0}\", using machine default", option);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine(
                    "Unknown code page \"{0}\", using machine default", option);
            }
            return 0;
        }
    }
}
