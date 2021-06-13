
// Uwaga: W wywołaniu generatora gppg należy użyć opcji /gplex

%namespace MiniCompiler

%union {
    public string val;
    public TypeEnum type;
    public SyntaxTree tree;
    public List<SyntaxTree> list;
}

%token Program If Else While Read Write Return Int Double Bool True False Hex
%token Assign Or And BitOr BitAnd Equal NotEqual Less LessEqual Greater GreaterEqual Plus Minus Multiply Divide Negate BitNegate
%token OpenPar ClosePar OpenBlock CloseBlock Comma Endline Eof
%token <val> Ident IntNumber DoubleNumber String Error

%type <tree> programContent declaration instruction output_instruction input_instruction block_instruction conditional_instruction
%type <tree> loop_instruction exp number
%type <type> type
%type <list> identifiers instructions declarations

%%

start
    : Program OpenBlock programContent CloseBlock Eof
        {
            Compiler.Program = new Program($3, @$);
            YYACCEPT;
        }
    | error Eof
        {
            Compiler.Error(@1);
            yyerrok();
            YYACCEPT;
        }
    ;
    
programContent
    : declarations instructions
        { $$ = new ProgramContent($1, $2, @$); }
    | declarations
        { $$ = new ProgramContent($1, null, @$); }
    | instructions
        { $$ = new ProgramContent(null, $1, @$); }
    |
        { $$ = new ProgramContent(null, null, @$); }
    ;

declarations
    : declarations declaration
        {
            $1.Add($2);
            $$ = $1;
        }
    | declaration
        {
            $$ = new List<SyntaxTree>();
            $$.Add($1);
        }
    ;
    
declaration
    : type identifiers Endline
        { $$ = new Declaration($1, $2, @$); }
    ;

type
    : Int
        { $$ = TypeEnum.Int; }
    | Double
        { $$ = TypeEnum.Double; }
    | Bool
        { $$ = TypeEnum.Bool; }
    ;

identifiers
    : identifiers Comma Ident 
        {
            $1.Add(new Identifier($3, @3));
            $$ = $1;
        }
    | Ident
        {
            $$ = new List<SyntaxTree>();
            $$.Add(new Identifier($1, @1));
        }
    ;

instructions
    : instructions instruction
        {
            $1.Add($2);
            $$ = $1;
        }
    | instruction
        {
            $$ = new List<SyntaxTree>();
            $$.Add($1);
        }
    ;
    
instruction 
    : output_instruction
    | input_instruction
    | block_instruction
    | conditional_instruction
    | loop_instruction
    | exp Endline
    | Return Endline
        { $$ = new ReturnInstruction(@1); }
    | error Endline
        {
            Compiler.Error(@1);
            $$ = null;
            yyerrok();
        }
    | error
        {
            Compiler.Error(@1);
            $$ = null;
            yyerrok();
        }
    | Eof
        {
            Compiler.Error(@1, "syntax error - unexpected end of file");
            YYABORT;
        }
    ;

conditional_instruction
    : If OpenPar exp ClosePar instruction
        { $$ = new ConditionalInstruction($3, $5, @$); }
    | If OpenPar exp ClosePar instruction Else instruction
        { $$ = new ConditionalInstruction($3, $5, $7, @$); }
    ;
        
loop_instruction
    : While OpenPar exp ClosePar instruction
        { $$ = new LoopInstruction($3, $5, @$); }
    ;
    
block_instruction
    : OpenBlock instructions CloseBlock
        { $$ = new BlockInstruction($2, @$); }
    | OpenBlock CloseBlock
        { $$ = null; }
    ;

exp
    : Ident Assign exp
        { $$ = new AssignmentExpression($1, $3, @$); }
    | Ident
        { $$ = new IdentifierExpression($1, @$); }
    | number
    ;
    
number
    : IntNumber
        { $$ = new NumberExpression($1, TypeEnum.Int, @$); }
    | DoubleNumber
        { $$ = new NumberExpression($1, TypeEnum.Double, @$); }
    | True
        { $$ = new NumberExpression("1", TypeEnum.Bool, @$); }
    | False
        { $$ = new NumberExpression("0", TypeEnum.Bool, @$); }
    ;

output_instruction
    : Write exp Endline
        { $$ = new OutputInstruction($2, OutputInstruction.Flag.None, @$); }
    | Write exp Comma Hex Endline
        { $$ = new OutputInstruction($2, OutputInstruction.Flag.Hex, @$); }
    | Write String Endline
        { $$ = new OutputInstruction($2, @$); }
    ;
    
input_instruction
    : Read Ident Endline
        { $$ = new InputInstruction(new Identifier($2, @2), false, @$); }
    | Read Ident Comma Hex Endline
        { $$ = new InputInstruction(new Identifier($2, @2), true, @$); }
    ;

%%

public Parser(Scanner scanner) : base(scanner) { }