
%using QUT.Gppg;
%namespace MiniCompiler

%x COMMENT
%x STRING
%s BLOCK
%s PARENTHESIS

Ident           [a-zA-z][a-zA-z0-9]*
IntNumber       ([1-9][0-9]*|0)
HexNumber       (0x|0X)[0-9a-fA-F]+
DoubleNumber    ([1-9][0-9]*|0)\.[0-9]+

%%

"program"       { return (int)Tokens.Program; }
"if"            { return (int)Tokens.If; }
"else"          { return (int)Tokens.Else; }
"while"         { return (int)Tokens.While; }
"read"          { return (int)Tokens.Read; }
"write"         { return (int)Tokens.Write; }
"return"        { return (int)Tokens.Return; }
"int"           { return (int)Tokens.Int; }
"double"        { return (int)Tokens.Double; }
"bool"          { return (int)Tokens.Bool; }
"true"          { return (int)Tokens.True; }
"false"         { return (int)Tokens.False; }
"hex"           { return (int)Tokens.Hex; }

{IntNumber}     { yylval.val = yytext; return (int)Tokens.IntNumber; }
{HexNumber}     { yylval.val = Convert.ToInt32(yytext, 16).ToString(); return (int)Tokens.IntNumber; }
{DoubleNumber}  { yylval.val = yytext; return (int)Tokens.DoubleNumber; }
{Ident}         { yylval.val = yytext; return (int)Tokens.Ident; }

"="             { return (int)Tokens.Assign; }
"||"            { return (int)Tokens.Or; }
"&&"            { return (int)Tokens.And; }
"|"             { return (int)Tokens.BitOr; }
"&"             { return (int)Tokens.BitAnd; }
"=="            { return (int)Tokens.Equal; }
"!="            { return (int)Tokens.NotEqual; }
">"             { return (int)Tokens.Greater; }
">="            { return (int)Tokens.GreaterEqual; }
"<"             { return (int)Tokens.Less; }
"<="            { return (int)Tokens.LessEqual; }
"+"             { return (int)Tokens.Plus; }
"-"             { return (int)Tokens.Minus; }
"*"             { return (int)Tokens.Multiply; }
"/"             { return (int)Tokens.Divide; }
"!"             { return (int)Tokens.Negate; }
"~"             { return (int)Tokens.BitNegate; }

"("             { parenthesis++; return (int)Tokens.OpenPar; }
")"             { 
                    if(--parenthesis < 0) {
                        Error("no matching opening parenthesis");
                        return (int)Tokens.Error;
                    } 
                    return (int)Tokens.ClosePar; 
                }
"{"			    { blocks++; return (int)Tokens.OpenBlock; }
"}"			    { 
                    if(--blocks < 0) {
                        Error("no matching opening bracket");
                        return (int)Tokens.Error;
                    } 
                    return (int)Tokens.CloseBlock; 
                }
","             { return (int)Tokens.Comma; }
";"             { 
                    if (parenthesis > 0) {
                        Error("unclosed parenthesis in instruction");
                        return (int)Tokens.Error;
                    }
                    return (int)Tokens.Endline; 
                }

" "             { }
"\t"            { }
"\r"            { }
"\n"            { }

<INITIAL>"//"   { BEGIN(COMMENT); }
<INITIAL>"\""   { BEGIN(STRING); parsedString = new StringBuilder(); stringLengthModif = 0; }

<<EOF>>         {
                    if (parenthesis > 0) {
                        Error("unclosed parenthesis");
                    } 
                    if (blocks > 0) {
                        Error("unclosed block");
                    }
                    return (int)Tokens.Eof; 
                }
.               { Error("unexpected character"); return (int)Tokens.Error; }

<COMMENT>"\n"   { BEGIN(INITIAL); }
<COMMENT>.      { }

<STRING>"\\\\"  { parsedString.Append(yytext); stringLengthModif--; }
<STRING>"\\n"   { parsedString.Append("\\0A"); stringLengthModif -= 2; }
<STRING>"\\\""  { parsedString.Append("\\22"); stringLengthModif -= 2; }
<STRING>"\""    { 
                    yylval.val = parsedString.ToString(); 
                    Compiler.AddLiteral(yylval.val, stringLengthModif);
                    BEGIN(INITIAL); 
                    return (int)Tokens.String; 
                }
<STRING>"\\"    { }
<STRING>\n      { Error("Newline in string now allowed"); BEGIN(INITIAL); return (int)Tokens.Error; }
<STRING><<EOF>> { Error("Unclosed string"); BEGIN(INITIAL); return (int)Tokens.Eof; }
<STRING>.       { parsedString.Append(yytext); }

%{
    yylloc = new LexLocation(tokLin,tokCol,tokELin,tokECol);
%}

%%

private void Error(string text) {
    Console.WriteLine(string.Format("Error ({0},{1}): {2}", yyline, yycol, text));
    Compiler.Errors++;
}

private int parenthesis = 0;
private int blocks = 0;

private StringBuilder parsedString;
private int stringLengthModif = 0;
