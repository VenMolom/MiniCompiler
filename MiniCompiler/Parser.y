
// Uwaga: W wywołaniu generatora gppg należy użyć opcji /gplex

%namespace MiniCompiler

%union {
    public string val;
    public TypeEnum type
    public SyntaxTree tree;
    public List<SyntaxTree> list;
}

%token Program If Else While Read Write Return Int Double Bool True False Hex
%token Assign Or And BitOr BitAnd Equal NotEqual Less LessEqual Greater GreaterEqual Plus Minus Multiply Divide Negate BitNegate
%token OpenPar ClosePar OpenBlock CloseBlock Comma Endline Eof
%token <val> Ident IntNumber DoubleNumber String Error

%type <tree> programContent declaration instruction output_instruction input_instruction block_instruction conditional_instruction
%type <tree> loop_instruction exp
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
            $$ = null;
            YYACCEPT;
        }
    ;
    
programContent
    : declarations instructions
        { $$ = new ProgramContent(declarations, instructions, @$); }
    | declarations
        { $$ = new ProgramContent(declarations, null, @$); }
    | instructions
        { $$ = new ProgramContent(null, instructions, @$); }
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
            $1.Add(new Identifier($1, @1));
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
        {
            $$ = new ReturnInstruction(@1);
        }
    | error Endline
        {
            Compiler.Error(@1);
            $$ = null;
            yyerrok();
        }
    | error
        {
            Error(@1);
            $$ = null;
            yyerrok();
        }
    | Eof
        {
            Error(@1, "syntax error - unexpected end of file");
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

// odtąd poprawić dalej
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
    | Write String Endline
        {
            PrintString($2);
        }
    ;
    
input_instruction
    : Read Ident Endline
        {
            var typeNullable = GetIdentifierType($2, @2);
            if (!typeNullable.HasValue) { return; }
            
            var type = typeNullable.Value;
            if (type == TypeEnum.Bool)
            {
                Error(@2, "cannot read into bool");
                return;
            }
        
            Read($2, type);
        }
    | Read Ident Comma Hex Endline
        {
            var typeNullable = GetIdentifierType($2, @2);
            if (!typeNullable.HasValue) { return; }
            
            var type = typeNullable.Value;
            if (type != TypeEnum.Int) 
            {
                Error(@2, "variable must be int to be read as hex");
                return;
            }
            
            ReadHex($2);
        }
    ;

%%

private TypeEnum? GetIdentifierType(string identifier, LexLocation location)
{
    if (!Compiler.Identifiers.ContainsKey(identifier))
    {
        Error(location, "undeclared variable");
        return null;
    }
    
    return Compiler.Identifiers[identifier];
}

private void Load(string target, string source, TypeEnum type)
{
    Compiler.EmitCode("{0} = load {1}, {1}* {2}", target, type.LLVMType(), source);
}

private void Store(string target, string source, TypeEnum type)
{
    Compiler.EmitCode("store {1} {0}, {1}* {2}", source, type.LLVMType(), target);
}

private void Print(string value, TypeEnum type)
{
    if (type == TypeEnum.Bool)
    {
        PrintBool(value);
        return;
    }

    var length = type == TypeEnum.Double ? 4 : 3;
    Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([{1} x i8]* @{0} to i8*), {0} {2})", 
        type.LLVMType(), length, value);
}

private void PrintBool(string value)
{
    var truelab = Compiler.NewLabel();
    var falselab = Compiler.NewLabel();
    var endlab = Compiler.NewLabel();
    Compiler.EmitCode("br i1 {0}, label %{1}, label %{2}", value, truelab, falselab);
    Compiler.EmitCode("{0}:", truelab);
    Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([5 x i8]* @true to i8*))");
    Compiler.EmitCode("br label %{0}", endlab);
    Compiler.EmitCode("{0}:", falselab);
    Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @false to i8*))");
    Compiler.EmitCode("br label %{0}", endlab);
    Compiler.EmitCode("{0}:", endlab);
}

private void PrintHex(string value)
{
    Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @hex to i8*), i32 {0})",value);
}

private void PrintString(string value)
{
    // TODO: ogarnąć tą funkcję by działało
    var temp = "@asd";
    var length = value.Length + 2;
    Compiler.EmitCode("{0} = constant [{1} x i8] c\"{2}\\0A\\00\"", temp, length, value);
    Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([{0} x i8]* {1} to i8*))", length, temp);
}

private void Read(string identifier, TypeEnum type)
{
    var length = type == TypeEnum.Double ? 4 : 3;
        Compiler.EmitCode("call i32 (i8*, ...) @scanf(i8* bitcast ([{1} x i8]* @{0} to i8*), {0}* {2})", 
            type.LLVMType(), length, identifier);
}

private void ReadHex(string identifier)
{
        Compiler.EmitCode("call i32 (i8*, ...) @scanf(i8* bitcast ([3 x i8]* @hexread to i8*), i32* {0})", identifier);
}

public Parser(Scanner scanner) : base(scanner) { }