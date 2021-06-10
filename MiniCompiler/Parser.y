
// Uwaga: W wywołaniu generatora gppg należy użyć opcji /gplex

%namespace MiniCompiler

%union {
    public string val;
    public TypeEnum type;
    public Exp exp;
}

%token Program If Else While Read Write Return Int Double Bool True False Hex
%token Assign Or And BitOr BitAnd Equal NotEqual Less LessEqual Greater GreaterEqual Plus Minus Multiply Divide Negate BitNegate
%token OpenPar ClosePar OpenBlock CloseBlock Comma Endline Eof
%token <val> Ident IntNumber DoubleNumber String Error

%type <type> type declaration
%type <exp> exp number

%%

start
    : Program OpenBlock programContent CloseBlock Eof
        {
            YYACCEPT;
        }
    | error Eof
        {
            Error(@1);
            yyerrok();
            YYACCEPT;
        }
    ;
    
programContent
    : declarations instructions
    | declarations
    | instructions
    ;

declarations
    : declarations declaration
    | declaration
    ;
    
declaration
    : type 
        {
            identifiersType = $1;
        }
      identifiers Endline
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
        { Declare($3, identifiersType, @3); }
    | Ident
        { Declare($1, identifiersType, @1); }
    ;

instructions
    : instructions instruction
    | instruction
    ;
    
instruction 
    : expression_instruction
    | output_instruction
    | error Endline
        {
            Error(@1);
            yyerrok();
        }
    | error
        {
            Error(@1);
            yyerrok();
        }
    | Eof
        {
            Error(@1, "syntax error - unexpected end of file");
            YYACCEPT;
        }
    ;
    
expression_instruction
    : exp Endline
    ;

exp
    : Ident Assign exp
        {
            var typeNullable = GetIdentifierType($1, @1);
            if (!typeNullable.HasValue) { return; }
            
            var type = typeNullable.Value;
            if (type == TypeEnum.Double && $3.Type == TypeEnum.Bool)
            {
                Error(@3, "cannot assign bool to double");
                return;
            }
            if (type == TypeEnum.Int && $3.Type != TypeEnum.Int)
            {
                Error(@3, "can only assign int to int");
                return;
            }
             if (type == TypeEnum.Bool && $3.Type != TypeEnum.Bool)
             {
                Error(@3, "can only assign bool to bool");
                return;
            }
            
            Store($1, $3.Identifier, type);
            
            var temp = Compiler.NewTemp();
            Load(temp, $1, type);
            $$ = new Exp(temp, type);
        }
    | Ident
        {
            var typeNullable = GetIdentifierType($1, @1);
            if (!typeNullable.HasValue) { return; }
            
            var type = typeNullable.Value;
            var temp = Compiler.NewTemp();
            Load(temp, $1, type);
            $$ = new Exp(temp, type);
        }
    | number
    ;
    
number
    : IntNumber
        {
            $$ = new Exp($1, TypeEnum.Int);
        }
    | DoubleNumber
        {
            $$ = new Exp($1, TypeEnum.Double);
        }
    | True
        {
            $$ = new Exp("1", TypeEnum.Bool);
        }
    | False
        {
            $$ = new Exp("0", TypeEnum.Bool);
        }
    ;

output_instruction
    : Write exp Endline
        {
            Print($2.Identifier, $2.Type);
        }
    | Write exp Comma Hex Endline
        {
            if ($2.Type != TypeEnum.Int) 
            {
                Error(@2, "variable must be int to be printed as hex");
                return;
            }
            
            PrintHex($2.Identifier);
        }
    ;

%%

private TypeEnum identifiersType;

private void Error(LexLocation location, string text = "syntax error")
{
    Console.WriteLine(string.Format("Error ({0},{1}): {2}", location.StartLine, location.StartColumn, text));
    Compiler.Errors++;
}

private void Declare(string identifier, TypeEnum type, LexLocation location)
{
    if (Compiler.Identifiers.ContainsKey(identifier))
    {
        Error(location, "variable already declared");
        return;
    }
    
    Compiler.Identifiers.Add(identifier, type);
    Alloca(identifier, type);
}

private TypeEnum? GetIdentifierType(string identifier, LexLocation location)
{
    if (!Compiler.Identifiers.ContainsKey(identifier))
    {
        Error(location, "undeclared variable");
    }
    
    return Compiler.Identifiers[identifier];
}

private void Load(string target, string source, TypeEnum type)
{
    Compiler.EmitCode("{0} = load {1}, {1}* {2}", target, type.LLVMType(), source);
}

private void Alloca(string identifier, TypeEnum type)
{
    Compiler.EmitCode("{0} = alloca {1}", identifier, type.LLVMType());
}

private void Store(string target, string source, TypeEnum type)
{
    Compiler.EmitCode("store {1} {0}, {1}* {2}", source, type.LLVMType(), target);
}

private void Print(string value, TypeEnum type)
{
    var length = type == TypeEnum.Double ? 5 : 4;
    Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([{1} x i8]* @{0} to i8*), {0} {2})", 
        type.LLVMType(), length, value);
}

private void PrintHex(string value)
{
    Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @hex to i8*), i32 {0})",value);
}

public Parser(Scanner scanner) : base(scanner) { }