
%namespace CLexer
%option noparser nofiles stack

LINE_COMMENT 		\/\/
START_COMMENT 		\/\*
END_COMMENT 		\*\/

%x LineComment
%x StreamComment


%%

// string constants

// comment parsing
{LINE_COMMENT}			if (YY_START != LineComment)   { Text.Append(yytext); Text.Append("[ "); yy_push_state(LineComment); }
{START_COMMENT}			if (YY_START != StreamComment) { Text.Append(yytext); Text.Append("[ "); yy_push_state(StreamComment); }

<LineComment> {
	\r					Text.Append(" ]"); Text.Append(yytext); yy_pop_state();
	.					Text.Append(yytext); // comment text
}
<StreamComment> {
	{END_COMMENT}		Text.Append(" ]"); Text.Append(yytext); yy_pop_state();
	<<EOF>>				{ throw new Exception("comment is unterminated"); }
	.					Text.Append(yytext); // comment text
}

<*>.					Text.Append(yytext);

%%

	public StringBuilder Text = new StringBuilder ();

