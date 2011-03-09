
%namespace CLexer
%option noparser nofiles stack

LINE_COMMENT 		\/\/
START_COMMENT 		\/\*
END_COMMENT 		\*\/

%x CMNT


%%

// string constants

// comment parsing
{LINE_COMMENT}			Text.Append(yytext); Text.Append("LC ");
{START_COMMENT}			if (YY_START != CMNT) { Text.Append(yytext); Text.Append("[ "); yy_push_state(CMNT); }
<CMNT>{END_COMMENT}		Text.Append(" ]"); Text.Append(yytext); yy_pop_state();
<CMNT>.					Text.Append(yytext); // comment text

<*>.					Text.Append(yytext);

%%

	public StringBuilder Text = new StringBuilder ();

