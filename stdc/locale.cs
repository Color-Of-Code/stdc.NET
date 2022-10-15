namespace stdc;

public partial class C
{

    public static readonly int LC_ALL = 0;
    public static readonly int LC_COLLATE = 1;
    public static readonly int LC_CTYPE = 2;
    public static readonly int LC_MONETARY = 3;
    public static readonly int LC_NUMERIC = 4;
    public static readonly int LC_TIME = 5;

    public static string setlocale(int category, string locale)
    {
        return null;
    }

    //struct lconv {
    //    char *decimal_point;
    //    char *thousands_sep;
    //    char *grouping;
    //    char *int_curr_symbol;
    //    char *currency_symbol;
    //    char *mon_decimal_point;
    //    char *mon_thousands_sep;
    //    char *mon_grouping;
    //    char *positive_sign;
    //    char *negative_sign;
    //    char int_frac_digits;
    //    char frac_digits;
    //    char p_cs_precedes;
    //    char p_sep_by_space;
    //    char n_cs_precedes;
    //    char n_sep_by_space;
    //    char p_sign_posn;
    //    char n_sign_posn;
    //    };

    //char *setlocale(int, const char *);
    //struct lconv * localeconv(void);
}
