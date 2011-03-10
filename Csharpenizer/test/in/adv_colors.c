//Display +200 colors, rgb values, their hex equivalents and names

#include <stdio.h>

#define TOTAL_COLORS   12

struct colour {
	char name[28];
	int red;
	int green;
	int blue;
};

struct colour whitetab[] = {
	{ "antique white",       250, 235, 215 },
	{ "azure",               240, 255, 255 },
	{ "bisque",              255, 228, 196 },
	{ "blanched almond",     255, 235, 205 },
	{ "cornsilk",            255, 248, 220 },
	{ "eggshell",            252, 230, 201 },
	{ "floral white",        255, 250, 240 },
	{ "gainsboro",           220, 220, 220 },
	{ "ghost white",         248, 248, 255 },
	{ "honeydew",            240, 255, 240 },
	{ "ivory",               255, 255, 240 },
	{ "lavender",            230, 230, 250 },
	{ "lavender blush",      255, 240, 245 },
	{ "lemon chiffon",       255, 250, 205 },
	{ "linen",               250, 240, 230 },
	{ "mint cream",          245, 255, 250 },
	{ "misty rose",          255, 228, 225 },
	{ "moccasin",            255, 228, 181 },
	{ "navajo white",        255, 222, 173 },
	{ "old lace",            253, 245, 230 },
	{ "papaya whip",         255, 239, 213 },
	{ "peach puff",          255, 218, 185 },
	{ "seashell",            255, 245, 238 },
	{ "snow",                255, 250, 250 },
	{ "thistle",             216, 191, 216 },
	{ "titanium white",      252, 255, 240 },
	{ "wheat",               245, 222, 179 },
	{ "white",               255, 255, 255 },
	{ "white smoke",         245, 245, 245 },
	{ "zinc white",          253, 248, 255 }
};

struct colour greytab[] = {
	{ "cold grey",           128, 138, 135 },
	{ "dim grey",            105, 105, 105 },
	{ "grey",                192, 192, 192 },
	{ "light grey",          211, 211, 211 },
	{ "slate grey",          112, 128, 144 },
	{ "slate grey dark",      47,  79,  79 },
	{ "slate grey light",    119, 136, 153 },
	{ "warm grey",           128, 128, 105 }
};

struct colour blacktab[] = {
	{ "black",       0,  0,  0  },
	{ "ivory black", 41, 36, 33 },
	{ "lamp black",  46, 71, 59 }
};

struct colour redtab[] = {
	{ "alizarin crimson",    227,  38,  54 },
	{ "brick",               156, 102,  31 },
	{ "cadmium red deep",    227,  23,  13 },
	{ "coral",               255, 127,  80 },
	{ "coral light",         240, 128, 128 },
	{ "deep pink",           255,  20, 147 },
	{ "english red",         212,  61,  26 },
	{ "firebrick",           178,  34,  34 },
	{ "geranium lake",       227,  18,  48 },
	{ "hot pink",            255, 105, 180 },
	{ "indian red",          176,  23,  31 },
	{ "light salmon",        255, 160, 122 },
	{ "madder lake deep",    227,  46,  48 },
	{ "maroon",              176,  48,  96 },
	{ "pink",                255, 192, 203 },
	{ "pink light",          255, 182, 193 },
	{ "raspberry",           135,  38,  87 },
	{ "red",                 255,   0,   0 },
	{ "rose madder",         227,  54,  56 },
	{ "salmon",              250, 128, 114 },
	{ "tomato",              255,  99,  71 },
	{ "venetian red",        212,  26,  31 }
};

struct colour browntab[] = {
	{ "beige",               163, 148, 128 },
	{ "brown",               128,  42,  42 },
	{ "brown madder",        219,  41,  41 },
	{ "brown ochre",         135,  66,  31 },
	{ "burlywood",           222, 184, 135 },
	{ "burnt sienna",        138,  54,  15 },
	{ "burnt umber",         138,  51,  36 },
	{ "chocolate",           210, 105,  30 },
	{ "deep ochre",          115,  61,  26 },
	{ "flesh",               255, 125,  64 },
	{ "flesh ochre",         255,  87,  33 },
	{ "gold ochre",          199, 120,  38 },
	{ "greenish umber",      255,  61,  13 },
	{ "khaki",               240, 230, 140 },
	{ "khaki dark",          189, 183, 107 },
	{ "light beige",         245, 245, 220 },
	{ "peru",                205, 133,  63 },
	{ "rosy brown",          188, 143, 143 },
	{ "raw sienna",          199,  97,  20 },
	{ "raw umber",           115,  74,  18 },
	{ "sepia",                94,  38,  18 },
	{ "sienna",              160,  82,  45 },
	{ "saddle brown",        139,  69,  19 },
	{ "sandy brown",         244, 164,  96 },
	{ "tan",                 210, 180, 140 },
	{ "van dyke brown",       94,  38,   5 }
};

struct colour orangetab[] = {
	{ "cadmium orange",    255,  97,   3 },
	{ "cadmium red_light", 255,   3,  13 },
	{ "carrot",            237, 145,  33 },
	{ "dark orange",       255, 140,   0 },
	{ "mars orange",       150,  69,  20 },
	{ "mars yellow",       227, 112,  26 },
	{ "orange",            255, 128,   0 },
	{ "orange red",        255,  69,   0 },
	{ "yellow ochre",      227, 130,  23 }
};

struct colour yellowtab[] = {
	{ "aureoline yellow",     255, 168,  36 },
	{ "banana",               227, 207,  87 },
	{ "cadmium lemon",        255, 227,   3 },
	{ "cadmium yellow",       255, 153,  18 },
	{ "cadmium yellow light", 255, 176,  15 },
	{ "gold",                 255, 215,   0 },
	{ "goldenrod",            218, 165,  32 },
	{ "goldenrod dark",       184, 134,  11 },
	{ "goldenrod light",      250, 250, 210 },
	{ "goldenrod pale",       238, 232, 170 },
	{ "light goldenrod",      238, 221, 130 },
	{ "melon",                227, 168, 105 },
	{ "naples yellow deep",   255, 168,  18 },
	{ "yellow",               255, 255,   0 },
	{ "yellow light",         255, 255, 224 }
};

struct colour greentab[] = {
	{ "chartreuse",          127, 255,   0 },
	{ "chrome oxide green",  102, 128,  20 },
	{ "cinnabar green",       97, 179,  41 },
	{ "cobalt green",         61, 145,  64 },
	{ "emerald green",         0, 201,  87 },
	{ "forest green",         34, 139,  34 },
	{ "green",                 0, 255,   0 },
	{ "green dark",            0, 100,   0 },
	{ "green pale",          152, 251, 152 },
	{ "green yellow",        173, 255,  47 },
	{ "lawn green",          124, 252,   0 },
	{ "lime green",           50, 205,  50 },
	{ "mint",                189, 252, 201 },
	{ "olive",                59,  94,  43 },
	{ "olive drab",          107, 142,  35 },
	{ "olive green dark",     85, 107,  47 },
	{ "permanent green",      10, 201,  43 },
	{ "sap green",            48, 128,  20 },
	{ "sea green",            46, 139,  87 },
	{ "sea green dark",      143, 188, 143 },
	{ "sea green medium",     60, 179, 113 },
	{ "sea green light",      32, 178, 170 },
	{ "spring green",          0, 255, 127 },
	{ "spring green medium",   0, 250, 154 },
	{ "terre verte",          56,  94,  15 },
	{ "viridian light",      110, 255, 112 },
	{ "yellow green",        154, 205,  50 },
};

struct colour cyantab[] = {
	{ "aquamarine",        127, 255, 212 },
	{ "aquamarine medium", 102, 205, 170 },
	{ "cyan",              000, 255, 255 },
	{ "cyan white",        224, 255, 255 },
	{ "turquoise",         064, 224, 208 },
	{ "turquoise dark",    000, 206, 209 },
	{ "turquoise medium",  072, 209, 204 },
	{ "turquoise pale",    175, 238, 238 }
};

struct colour bluetab[] = {
	{ "alice blue",        240, 248, 255 },
	{ "blue",                0,   0, 255 },
	{ "blue light",        173, 216, 230 },
	{ "blue medium",         0,   0, 205 },
	{ "cadet",              95, 158, 160 },
	{ "cobalt",             61,  89, 171 },
	{ "cornflower",        100, 149, 237 },
	{ "cerulean",            5, 184, 204 },
	{ "dodger blue",        30, 144, 255 },
	{ "indigo",              8,  46,  84 },
	{ "manganese blue",      3, 168, 158 },
	{ "midnight blue",      25,  25, 112 },
	{ "navy",                0,   0, 128 },
	{ "peacock",            51, 161, 201 },
	{ "powder blue",       176, 224, 230 },
	{ "royal blue",         65, 105, 225 },
	{ "slate blue",        106,  90, 205 },
	{ "slate blue dark",    72,  61, 139 },
	{ "slate blue light",  132, 112, 255 },
	{ "slate blue medium", 123, 104, 238 },
	{ "sky blue",          135, 206, 235 },
	{ "sky blue deep",       0, 191, 255 },
	{ "sky blue light",    135, 206, 250 },
	{ "steel blue",         70, 130, 180 },
	{ "steel blue light",  176, 196, 222 },
	{ "turquoise blue",      0, 199, 140 },
	{ "ultramarine",        18,  10, 143 }
};

struct colour magentatab[] = {
	{ "blue violet",          138,  43, 226 },
	{ "cobalt violet deep",   145,  33, 158 },
	{ "magenta",              255,   0, 255 },
	{ "orchid",               218, 112, 214 },
	{ "orchid dark",          153,  50, 204 },
	{ "orchid medium",        186,  85, 211 },
	{ "permanent red violet", 219,  38,  69 },
	{ "plum",                 221, 160, 221 },
	{ "purple",               160,  32, 240 },
	{ "purple medium",        147, 112, 219 },
	{ "ultramarine violet",    92,  36, 110 },
	{ "violet",               143,  94, 153 },
	{ "violet dark",          148,   0, 211 },
	{ "violet red",           208,  32, 144 },
	{ "violet red medium",    199,  21, 133 },
	{ "violet red pale",      219, 112, 147 }
};


struct colour namedtab[] = {
	{ "aqua",      0, 255, 255 },
	{ "black",     0,   0,   0 },
	{ "blue",      0,   0, 255 },
	{ "fuchsia", 255,   0, 255 },
	{ "gray",    128, 128, 128 },
	{ "green",     0, 128,   0 },
	{ "lime",      0, 255,   0 },
	{ "maroon",  128,   0,   0 },
	{ "navy",      0,   0, 128 },
	{ "olive",   128, 128,   0 },
	{ "purple",  128,   0, 128 },
	{ "red",     255,   0,   0 },
	{ "silver",  192, 192, 192 },
	{ "teal",      0, 128, 128 },
	{ "white",   255, 255, 255 },
	{ "yellow",  255, 255,   0 }
};

struct tabs {
	struct colour *table;
	int tabsize;
} coltab[TOTAL_COLORS];

int main(void) {
	enum colours { WHITE, GREY, BLACK, RED, BROWN, ORANGE, YELLOW, 
		GREEN, CYAN, BLUE, MAGENTA, NAMED } color;

	char *colourname[] = { "white", "grey", "black", "red", "brown", "orange", 
		"yellow", "green", "cyan", "blue", "magenta", 
		"named colors" };
	int i = 0;
	int red = 0, green = 0, blue = 0;

	coltab[WHITE].table = whitetab;
	coltab[WHITE].tabsize = sizeof(whitetab) / sizeof(whitetab[0]);
	coltab[GREY].table = greytab;
	coltab[GREY].tabsize = sizeof(greytab) / sizeof(greytab[0]);
	coltab[BLACK].table = blacktab;
	coltab[BLACK].tabsize = sizeof(blacktab) / sizeof(blacktab[0]);
	coltab[RED].table = redtab;
	coltab[RED].tabsize = sizeof(redtab) / sizeof(redtab[0]);
	coltab[BROWN].table = browntab;
	coltab[BROWN].tabsize = sizeof(browntab) / sizeof(browntab[0]);
	coltab[ORANGE].table = orangetab;
	coltab[ORANGE].tabsize = sizeof(orangetab) / sizeof(orangetab[0]);
	coltab[YELLOW].table = yellowtab;
	coltab[YELLOW].tabsize = sizeof(yellowtab) / sizeof(yellowtab[0]);
	coltab[GREEN].table = greentab;
	coltab[GREEN].tabsize = sizeof(greentab) / sizeof(greentab[0]);
	coltab[CYAN].table = cyantab;
	coltab[CYAN].tabsize = sizeof(cyantab) / sizeof(cyantab[0]);
	coltab[BLUE].table = bluetab;
	coltab[BLUE].tabsize = sizeof(bluetab) / sizeof(bluetab[0]);
	coltab[MAGENTA].table = magentatab;
	coltab[MAGENTA].tabsize = sizeof(magentatab) / sizeof(magentatab[0]);
	coltab[NAMED].table = namedtab;
	coltab[NAMED].tabsize = sizeof(namedtab) / sizeof(namedtab[0]);

	for(color = WHITE; color < TOTAL_COLORS; color++) {
		printf("\n ==> %s <==\n", colourname[color]);
		for(i = 0; i < coltab[color].tabsize; i++) {
			red   = coltab[color].table[i].red;
			green = coltab[color].table[i].green;
			blue  = coltab[color].table[i].blue;

			printf(" %03d %03d %03d - #%02x%02x%02x - %s\n", 
				red, green, blue, red, green, blue, coltab[color].table[i].name);
		}
	}

	return 0;
}
