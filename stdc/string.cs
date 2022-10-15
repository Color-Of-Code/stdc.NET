namespace stdc;

using System;
using System.Text;

public partial class C
{

    //void * memchr(const void *, int, size_t);
    //int memcmp(const void *, const void *, size_t);
    //void * memcpy(void *, const void *, size_t);
    //void * memmove(void *, const void *, size_t);
    //void * memset(void *, int, size_t);

    /// <summary>
    /// 		char * strcat ( char * destination, const char * source );
    ///
    /// Concatenate strings
    /// Appends a copy of the source string to the destination string. The terminating
    /// null character in destination is overwritten by the first character of source,
    /// and a new null-character is appended at the end of the new string formed by the
    /// concatenation of both in destination.
    ///
    /// Parameters
    ///  destination
    ///   Pointer to the destination array, which should contain a C string, and be
    ///   large enough to contain the concatenated resulting string.
    ///
    ///  source
    ///   C string to be appended. This should not overlap destination.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static StringBuilder strcat(StringBuilder destination, String source)
    {
        destination.Append(source);
        return destination;
    }
    public static char[] strcat(char[] destination, String source)
    {
        int dstlen = strlen(destination);

        char[] src = source.ToCharArray();
        Array.Copy(src, 0, destination, dstlen, src.Length);
        return destination;
    }

    /// <summary>
    ///		char * strcpy ( char * destination, const char * source );
    ///
    /// Copy string
    /// Copies the C string pointed by source into the array pointed by
    /// destination, including the terminating null character.
    ///
    /// To avoid overflows, the size of the array pointed by destination shall be
    /// long enough to contain the same C string as source (including the terminating
    /// null character), and should not overlap in memory with source.
    ///
    /// Parameters
    ///  destination
    ///   Pointer to the destination array where the content is to be copied.
    ///
    ///  source
    ///   C string to be copied.
    ///
    /// Return Value
    ///  destination is returned.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static StringBuilder strcpy(StringBuilder destination, string source)
    {
        destination.Length = 0;
        return strcat(destination, source);
    }
    public static char[] strcpy(char[] destination, string source)
    {
        char[] src = source.ToCharArray();
        Array.Copy(src, destination, src.Length);
        destination[src.Length] = '\0';
        return destination;
    }

    /// <summary>
    /// 		char * strncpy ( char * destination, const char * source, size_t num );
    ///
    /// Copy characters from string
    /// Copies the first num characters of source to destination. If the end of the source
    /// C string (which is signaled by a null-character) is found before num characters have
    /// been copied, destination is padded with zeros until a total of num characters have
    /// been written to it.
    ///
    /// No null-character is implicitly appended to the end of destination, so destination
    /// will only be null-terminated if the length of the C string in source is less than num.
    ///
    /// Parameters
    ///  destination
    ///   Pointer to the destination array where the content is to be copied.
    ///
    ///  source
    ///   C string to be copied.
    ///
    ///  num
    ///   Maximum number of characters to be copied from source.
    ///
    /// Return Value
    ///  destination is returned.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="source"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static StringBuilder strncpy(StringBuilder destination, string source, int num)
    {
        destination.Length = 0;
        if (source.Length >= num)
            destination.Append(source, 0, num);
        else
            destination.Append(source);
        return destination;
    }
    public static char[] strncpy(char[] destination, string source, int num)
    {
        if (source.Length >= num)
        {
            Array.Copy(source.ToCharArray(), destination, num);
        }
        else
        {
            Array.Copy(source.ToCharArray(), destination, source.Length);
            Array.Clear(destination, source.Length, num - source.Length);
        }
        return destination;
    }

    /// <summary>
    ///		size_t strlen ( const char * str );
    ///
    /// Get string length
    /// Returns the length of str.
    ///
    /// The length of a C string is determined by the terminating null-character:
    /// A C string is as long as the amount of characters between the beginning of
    /// the string and the terminating null character.
    ///
    /// This should not be confused with the size of the array that holds the string.
    /// For example:
    ///		char mystr[100]="test string";
    /// defines an array of characters with a size of 100 chars, but the C string
    /// with which mystr has been initialized has a length of only 11 characters.
    /// Therefore, while sizeof(mystr) evaluates to 100, strlen(mystr) returns 11.
    ///
    /// Parameters
    ///  string
    ///   C string.
    ///
    /// Return Value
    ///   The length of string.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int strlen(char[] str)
    {
        int dstlen = 0;
        while (str[dstlen] != '\0')
            dstlen++;
        return dstlen;
    }

    public static int strlen(string str)
    {
        return str.Length;
    }


    //char * strncat(char *, const char *, size_t);
    //char * strchr(const char *, int);
    //int strcmp(const char *, const char *);
    //int strcoll(const char *, const char *);
    //size_t strxfrm(char *, const char *, size_t);
    //size_t strcspn(const char *, const char *);
    //char * strerror(int);
    //int strncmp(const char *, const char *, size_t);
    //char * strpbrk(const char *, const char *);
    //char * strrchr(const char *, int);
    //size_t strspn(const char *, const char *);
    //char * strstr(const char *, const char *);
    //char * strtok(char *, const char *);

}
