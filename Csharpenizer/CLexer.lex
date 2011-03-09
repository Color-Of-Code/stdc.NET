
%namespace CLexer
%option noparser nofiles stack

LINE_COMMENT 		\/\/
START_COMMENT 		\/\*
END_COMMENT 		\*\/
WS					[ \t]
STAR				[*]

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

// C stuff
FILE{WS}*{STAR}			{ Text.Append("C.FILE"); }

// C functions
fclose			|
fopen			|
fprintf			|
fputc			|
fputs			|
gets			|
printf			|
puts			|
qsort			|
rand			|
scanf			|
srand			|
strcat			|
strcpy			|
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

	public StringBuilder Text = new StringBuilder ();

