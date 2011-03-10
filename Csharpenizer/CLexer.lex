
%namespace CLexer
%using System.Text.RegularExpressions;
%option noparser nofiles stack

LINE_COMMENT 		\/\/
START_COMMENT 		\/\*
END_COMMENT 		\*\/
WS					[ \t]
STAR				[*]
LETTER				[a-zA-Z_]
DIGIT				[0-9]
IDENTIFIER			{LETTER}({LETTER}|{DIGIT})*
INTEGER				{DIGIT}+
CONST				(const{WS}+)

%x StringLiteral
%x LineComment
%x StreamComment


%%

// string constants
\"						{ /*Text.Append("<str>");*/ Text.Append(yytext); yy_push_state(StringLiteral); }

// comment parsing
{LINE_COMMENT}			{ /*Text.Append("<cmt>");*/ Text.Append(yytext); yy_push_state(LineComment); }
{START_COMMENT}			{ /*Text.Append("<cmt>");*/ Text.Append(yytext); yy_push_state(StreamComment); }

// here we go with preprocessor stuff
#[^\r\n]*				{ Text.Append("// "); Text.Append(yytext); }

// try to recognize main
int{WS}+main			{ ContainsIMain = true; Text.Append(yytext); }
void{WS}+main			{ ContainsVMain = true; Text.Append(yytext); }

// C stuff
FILE{WS}*{STAR}					{ Text.Append("C.FILE"); }
{CONST}?void{WS}+{STAR}			{ Text.Append("object"); }
&{WS}*{IDENTIFIER}									{ Text.Append(Regex.Replace(yytext, @"&", "out ")); }

// casts
{STAR}{WS}*\({WS}*{IDENTIFIER}{WS}*{STAR}{WS}*\)	{ /* remove casts */ }
//{STAR}{WS}*\({WS}*{IDENTIFIER}{WS}*{STAR}{WS}*\)	{ Text.Append(Regex.Replace(yytext, @".*?\(([^*]*).*", "($1)")); }


// array declaration transformation
{CONST}?char{WS}+{IDENTIFIER}{WS}*\[{WS}*\]{WS}*=	{ Text.Append(Regex.Replace(yytext, @".*?char\s+(\S+)\s*\[.*?\]", "string $1")); }
{CONST}?int{WS}+{IDENTIFIER}{WS}*\[[^\]]*\]{WS}*=	{ Text.Append(Regex.Replace(yytext, @".*?int\s+(\S+)\s*\[.*?\]\s*=", "int[] $1 = new int[]")); }
char{WS}+{IDENTIFIER}{WS}*\[[^\]]*\];				{ Text.Append(Regex.Replace(yytext, @".*?char\s+(\S+)\s*\[(.*?)\]", "char[] $1 = new char[$2]")); }

// C functions
exit			|
fclose			|
fgetc			|
fgets			|
fopen			|
fprintf			|
fputc			|
fputs			|
getch			|
gets			|
printf			|
putc			|
puts			|
qsort			|
rand			|
raise			|
scanf			|
signal			|
sprintf			|
srand			|
strcat			|
strcpy			|
strlen			|
time			|
NULL					{ Text.Append("C."); Text.Append(yytext); }


<StringLiteral> {
	\\\"				Text.Append(yytext); // ignore escaped "
	\"					Text.Append(yytext); /*Text.Append("</str>");*/ yy_pop_state();
	.					Text.Append(yytext); // literal text
}
<LineComment> {
	\r					/*Text.Append("</cmt>");*/ Text.Append(yytext); yy_pop_state();
	.					Text.Append(yytext); // comment text
}
<StreamComment> {
	{END_COMMENT}		Text.Append(yytext); /*Text.Append("</cmt>");*/ yy_pop_state();
	<<EOF>>				{ throw new Exception("comment is unterminated"); }
	.					Text.Append(yytext); // comment text
}

<*>.					Text.Append(yytext);

%%

	public Boolean ContainsIMain = false;
	public Boolean ContainsVMain = false;
	public StringBuilder Text = new StringBuilder ();

