using System;
using System.Collections.Generic;
using System.IO;
using QUT.Gppg;

namespace MiniCompiler
{ 
    public static class Compiler
    {
        public static readonly Dictionary<string, (string ident, TypeEnum type)> Symbols =
            new Dictionary<string, (string ident, TypeEnum type)>();
        public static readonly Dictionary<string, (string ident, int length)> StringLiterals =
            new Dictionary<string, (string, int)>();
        public static SyntaxTree Program = null;
        public static int Errors = 0;

        public static string RootLabel = "0";

        private static StreamWriter sw;
        private static int temps = 0;
        private static int literals = 0;

        public static int Main(string[] args)
        {
            Console.WriteLine("\nMini Language Compiler\n");

            string file;
            FileStream source;
            if (args.Length >= 1)
            {
                file = args[0];
            }
            else
            {
                Console.Write("source file: ");
                file = Console.ReadLine();
            }
            
            try
            {
                source = new FileStream(file, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
            
            Scanner scanner = new Scanner(source);
            Parser parser = new Parser(scanner);
            
            sw = new StreamWriter(file + ".ll");

            try
            {
                parser.Parse();
                if (Errors == 0 && Program != null)
                {   
                    GenProlog();
                    Program.GenerateCode();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error occured:\n {e.Message}");
                Errors++;
            }
            
            sw.Close();
            source.Close();
            
            if (Errors != 0 || Program == null)
            {
                if (Program == null && Errors == 0)
                {
                    Console.WriteLine("Program keyword not found");
                }
                
                if (Errors != 0)
                {
                    Console.WriteLine($"{Errors} Errors detected");
                }

                File.Delete(file + ".ll");
                return 2;
            }
            
            Console.WriteLine("Compilation successful");
            return 0;
        }

        public static string NewTemp(bool label = false)
        {
            var prefix = label ? string.Empty : "%";
            return $"{prefix}__{++temps}";
        }
        
        public static string NewLiteral()
        {
            return $".str.{++literals}";
        }

        public static void EmitCode(string instr = null)
        {
            sw.WriteLine(instr);
        }

        public static void Error(LexLocation location, string text = "syntax error")
        {
            Console.WriteLine($"Error ({location.StartLine},{location.StartColumn}): {text}");
            Errors++;
        }

        public static void AddLiteral(string literal, int lengthModif)
        {
            if (!StringLiterals.ContainsKey(literal))
            {
                StringLiterals.Add(literal, (NewLiteral(), literal.Length + lengthModif + 1));
            }
        }

        private static void GenProlog()
        {
            EmitCode("; prolog");
            EmitCode("@i32 = constant [3 x i8] c\"%d\\00\"");
            EmitCode("@hex = constant [5 x i8] c\"0X%X\\00\"");
            EmitCode("@hexread = constant [3 x i8] c\"%X\\00\"");
            EmitCode("@double = constant [4 x i8] c\"%lf\\00\"");
            EmitCode("@true = constant [5 x i8] c\"True\\00\"");
            EmitCode("@false = constant [6 x i8] c\"False\\00\"");
            EmitCode();

            foreach (var stringLiteral in StringLiterals)
            {
                EmitCode($"@{stringLiteral.Value.ident} = constant [{stringLiteral.Value.length} x i8] " +
                         $"c\"{stringLiteral.Key}\\00\"");
            }
            
            EmitCode();
            EmitCode("declare i32 @printf(i8*, ...)");
            EmitCode("declare i32 @scanf(i8*, ...)");
            EmitCode();
        }
    }
    
    public enum TypeEnum {
        Int, Double, Bool, String
    }

    public static class TypeEnumExtension
    {
        public static string LlvmType(this TypeEnum type)
        {
            switch (type)
            {
                case TypeEnum.Bool:
                    return "i1";
                case TypeEnum.Double:
                    return "double";
                case TypeEnum.Int:
                    return "i32";
                case TypeEnum.String:
                    return "str";
                default:
                    return string.Empty;
            }
        }
    }

    public abstract class SyntaxTree
    {
        public LexLocation Location { get; }
        public TypeEnum Type { get; set; }
        public string Identifier { get; set; }
        
        public string ExitLabel { get; set; }

        protected SyntaxTree(LexLocation location)
        {
            Location = location;
        }

        public abstract void GenerateCode();
        
        protected void Load(string source)
        {
            Compiler.EmitCode($"{Identifier} = load {Type.LlvmType()}, {Type.LlvmType()}* {source}");
        }

        protected void Store(string target)
        {
            Compiler.EmitCode($"store {Type.LlvmType()} {Identifier}, {Type.LlvmType()}* {target}");
        }

        protected void Cast(string source, TypeEnum sourceType, string target, TypeEnum targetType)
        {
            Compiler.EmitCode($"{target} = sitofp {sourceType.LlvmType()} {source} to {targetType.LlvmType()}");
        }
    }

    public class Program : SyntaxTree
    {
        private readonly SyntaxTree content;

        public Program(SyntaxTree content, LexLocation location) : base(location)
        {
            this.content = content;
        }
        
        public override void GenerateCode()
        {
            Compiler.EmitCode("define i32 @main() #0");
            Compiler.EmitCode("{");
            Compiler.EmitCode();
            content?.GenerateCode();
            Compiler.EmitCode();
            Compiler.EmitCode("ret i32 0");
            Compiler.EmitCode("}");
        }
    }

    public class ProgramContent : SyntaxTree
    {
        private readonly List<SyntaxTree> declarations;
        private readonly List<SyntaxTree> instructions;
        
        public ProgramContent(List<SyntaxTree> declarations, List<SyntaxTree> instructions, LexLocation location)
            : base(location)
        {
            this.declarations = declarations;
            this.instructions = instructions;
        }

        public override void GenerateCode()
        {
            declarations?.ForEach(declaration => declaration?.GenerateCode());
            instructions?.ForEach(instruction => instruction?.GenerateCode());
        }
    }
    
    public class Identifier : SyntaxTree
    {
        public Identifier(string identifier, LexLocation location) : base(location)
        {
            Identifier = identifier;
        }

        public override void GenerateCode()
        {
        }
    }

    public class Declaration : SyntaxTree
    {
        private readonly List<SyntaxTree> identifiers;
        
        public Declaration(TypeEnum type, List<SyntaxTree> identifiers, LexLocation location) : base(location)
        {
            Type = type;
            this.identifiers = identifiers;
        }

        public override void GenerateCode()
        {
            identifiers?.ForEach(s => Declare(s.Identifier, Type, s.Location));
        }
        
        private void Declare(string identifier, TypeEnum type, LexLocation location)
        {
            if (Compiler.Symbols.ContainsKey(identifier))
            {
                Compiler.Error(location, "variable already declared");
                return;
            }

            var ident = Compiler.NewTemp();
            Compiler.Symbols.Add(identifier, (ident, type));
            Compiler.EmitCode($"{ident} = alloca {type.LlvmType()}");
        }
    }
    
    public class ReturnInstruction : SyntaxTree
    {
        public ReturnInstruction(LexLocation location) : base(location)
        {
        }

        public override void GenerateCode()
        {
            Compiler.EmitCode("ret i32 0");
        }
    }

    public class ConditionalInstruction : SyntaxTree
    {
        private readonly SyntaxTree condition;
        private readonly SyntaxTree thenInstruction;
        private readonly SyntaxTree elseInstruction;
        
        public ConditionalInstruction(SyntaxTree condition, SyntaxTree thenInstruction, LexLocation location)
            : base(location)
        {
            this.condition = condition;
            this.thenInstruction = thenInstruction;
        }
        
        public ConditionalInstruction(SyntaxTree condition, SyntaxTree thenInstruction, SyntaxTree elseInstruction,
                                      LexLocation location)
            : this(condition, thenInstruction, location)
        {
            this.elseInstruction = elseInstruction;
        }

        public override void GenerateCode()
        {
            var truelab = Compiler.NewTemp(true);
            var falselab = Compiler.NewTemp(true);
            var endlab = Compiler.NewTemp(true);
            
            condition.GenerateCode();
            
            if (condition.Type != TypeEnum.Bool)
            {
                Compiler.Error(condition.Location, "condition needs to be of type bool");
                return;
            }
            
            Compiler.EmitCode($"br i1 {condition.Identifier}, label %{truelab}, label %{falselab}");
            Compiler.EmitCode($"{truelab}:");
            Compiler.RootLabel = truelab;
            thenInstruction?.GenerateCode();
            Compiler.EmitCode($"br label %{endlab}");
            Compiler.EmitCode($"{falselab}:");
            Compiler.RootLabel = falselab;
            elseInstruction?.GenerateCode();
            Compiler.EmitCode($"br label %{endlab}");
            Compiler.EmitCode($"{endlab}:");
            Compiler.RootLabel = endlab;
        }
    }
    
    public class LoopInstruction : SyntaxTree
    {
        private readonly SyntaxTree condition;
        private readonly SyntaxTree instruction;
        
        public LoopInstruction(SyntaxTree condition, SyntaxTree instruction, LexLocation location) : base(location)
        {
            this.condition = condition;
            this.instruction = instruction;
        }

        public override void GenerateCode()
        {
            var startlab = Compiler.NewTemp(true);
            var innerlab = Compiler.NewTemp(true);
            var endlab = Compiler.NewTemp(true);
            
            Compiler.EmitCode($"br label %{startlab}");
            Compiler.EmitCode($"{startlab}:");
            Compiler.RootLabel = startlab;
            condition.GenerateCode();
            
            if (condition.Type != TypeEnum.Bool)
            {
                Compiler.Error(condition.Location, "condition needs to be of type bool");
                return;
            }
            
            Compiler.EmitCode($"br i1 {condition.Identifier}, label %{innerlab}, label %{endlab}");
            Compiler.EmitCode($"{innerlab}:");
            Compiler.RootLabel = innerlab;
            instruction?.GenerateCode();
            Compiler.EmitCode($"br label %{startlab}");
            Compiler.EmitCode($"{endlab}:");
            Compiler.RootLabel = endlab;
        }
    }
    
    public class BlockInstruction : SyntaxTree
    {
        private readonly List<SyntaxTree> instructions;
        
        public BlockInstruction(List<SyntaxTree> instructions, LexLocation location) : base(location)
        {
            this.instructions = instructions;
        }

        public override void GenerateCode()
        {
            instructions?.ForEach(instruction => instruction?.GenerateCode());
        }
    }
    
    public class OutputInstruction : SyntaxTree
    {
        private readonly SyntaxTree expression;
        private readonly Flag flag;
        private readonly string literal;
        
        public OutputInstruction(SyntaxTree expression, Flag flag, LexLocation location) : base(location)
        {
            this.expression = expression;
            this.flag = flag;
        }

        public OutputInstruction(string literal, LexLocation location) : base(location)
        {
            this.literal = literal;
        }

        public override void GenerateCode()
        {
            if (expression == null && !string.IsNullOrEmpty(literal))
            {
                var (ident, length) = Compiler.StringLiterals[literal];
                PrintString(ident, length);
                return;
            }

            if (expression == null)
            {
                return;
            }
            
            expression.GenerateCode();
            if (flag == Flag.None)
            {
                if (expression.Type == TypeEnum.Bool)
                {
                    PrintBool();
                    return;
                }

                var length = expression.Type == TypeEnum.Double ? 4 : 3;
                Print(length);
            }
            else if (flag == Flag.Hex)
            {
                if (expression.Type != TypeEnum.Int)
                {
                    Compiler.Error(expression.Location, "variable must be int to be printed as hex");
                    return;
                }

                Print(5, "hex");
            }
        }

        private void Print(int length, string format = null)
        {
            Compiler.EmitCode($"call i32 (i8*, ...) @printf(i8* bitcast ([{length} x i8]* " +
                              $"@{format ?? expression.Type.LlvmType()} to i8*)" +
                              $", {expression.Type.LlvmType()} {expression.Identifier})");
        }
        private void PrintBool()
        {
            var truelab = Compiler.NewTemp(true);
            var falselab = Compiler.NewTemp(true);
            var endlab = Compiler.NewTemp(true);
            
            Compiler.EmitCode($"br i1 {expression.Identifier}, label %{truelab}, label %{falselab}");
            Compiler.EmitCode($"{truelab}:");
            Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([5 x i8]* @true to i8*))");
            Compiler.EmitCode($"br label %{endlab}");
            Compiler.EmitCode($"{falselab}:");
            Compiler.EmitCode("call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @false to i8*))");
            Compiler.EmitCode($"br label %{endlab}");
            Compiler.EmitCode($"{endlab}:");
            Compiler.RootLabel = endlab;
        }

        private void PrintString(string identifier, int length)
        {
            Compiler.EmitCode($"call i32 (i8*, ...) @printf(i8* bitcast ([{length} x i8]* " +
                              $"@{identifier} to i8*))");
        }

        public enum Flag
        {
            Hex, None
        }
    }

    public class InputInstruction : SyntaxTree
    {
        private readonly SyntaxTree identifier;
        private readonly bool hex;
        
        public InputInstruction(SyntaxTree identifier, bool hex, LexLocation location) : base(location)
        {
            this.identifier = identifier;
            this.hex = hex;
        }

        public override void GenerateCode()
        {
            if (!Compiler.Symbols.ContainsKey(identifier.Identifier))
            {
                Compiler.Error(identifier.Location, "undeclared variable");
                return;
            }

            var (ident, type) = Compiler.Symbols[identifier.Identifier];
            if (hex)
            {
                if (type != TypeEnum.Int)
                {
                    Compiler.Error(Location, "variable must be int to be read as hex");
                    return;
                }
                
                Read(ident, TypeEnum.Int.LlvmType(), 3, "hexread");
            }
            
            if (type == TypeEnum.Bool)
            {
                Compiler.Error(Location, "cannot read into bool");
                return;
            }
            
            var length = type == TypeEnum.Double ? 4 : 3;
            Read(ident, type.LlvmType(), length);
        }
        
        private void Read(string ident, string type, int length, string format = null)
        {
            Compiler.EmitCode($"call i32 (i8*, ...) @scanf(i8* bitcast ([{length} x i8]* " +
                              $"@{format ?? type} to i8*)" +
                              $", {type}* {ident})");
        }
    }

    public class IdentifierExpression : SyntaxTree
    {
        private readonly string identifier;

        public IdentifierExpression(string identifier, LexLocation location) : base(location)
        {
            this.identifier = identifier;
        }

        public override void GenerateCode()
        {
            if (!Compiler.Symbols.ContainsKey(identifier))
            {
                Compiler.Error(Location, "undeclared variable");
                return;
            }

            string ident;
            (ident, Type) = Compiler.Symbols[identifier];
            Identifier = Compiler.NewTemp();
            Load(ident);
        }
    }
    
    public class AssignmentExpression : SyntaxTree
    {
        private readonly string identifier;
        private readonly SyntaxTree expression;

        public AssignmentExpression(string identifier, SyntaxTree expression, LexLocation location) : base(location)
        {
            this.identifier = identifier;
            this.expression = expression;
        }

        public override void GenerateCode()
        {
            expression.GenerateCode();
            
            if (!Compiler.Symbols.ContainsKey(identifier))
            {
                Compiler.Error(Location, "undeclared variable");
                return;
            }

            string ident;
            (ident, Type) = Compiler.Symbols[identifier];
            switch (Type)
            {
                case TypeEnum.Bool when expression.Type != TypeEnum.Bool:
                    Compiler.Error(Location, "Can only assign bool to bool");
                    return;
                case TypeEnum.Int when expression.Type != TypeEnum.Int:
                    Compiler.Error(Location, "Can only assign int to int");
                    return;
                case TypeEnum.Double when expression.Type == TypeEnum.Bool:
                    Compiler.Error(Location, "Cannot assign bool to double");
                    return;
                case TypeEnum.String:
                    break;
            }

            if (Type == TypeEnum.Double && expression.Type == TypeEnum.Int)
            {
                var tmp = Compiler.NewTemp();
                Identifier = tmp;
                Cast(expression.Identifier, expression.Type);
                Store(ident);
            }
            else
            {
                Identifier = expression.Identifier;
                Store(ident);
            }
        }
        
        private void Cast(string source, TypeEnum sourceType)
        {
            Compiler.EmitCode($"{Identifier} = sitofp {sourceType.LlvmType()} {source} to {Type.LlvmType()}");
        }
    }
    
    public class NumberExpression : SyntaxTree
    {
        public NumberExpression(string value, TypeEnum type, LexLocation location) : base(location)
        {
            Type = type;
            Identifier = value;
        }

        public override void GenerateCode()
        {
        }
    }

    public class LogicalExpression : SyntaxTree
    {
        private readonly SyntaxTree left;
        private readonly SyntaxTree right;
        private readonly Operation operation;

        public LogicalExpression(SyntaxTree left, SyntaxTree right, Operation operation, LexLocation location)
            : base(location)
        {
            this.left = left;
            this.right = right;
            this.operation = operation;
        }

        public override void GenerateCode()
        {
            left.GenerateCode();
            if (left.Type != TypeEnum.Bool)
            {
                Compiler.Error(left.Location, "expression is not of type bool");
                return;
            }

            var rightlab = Compiler.NewTemp(true);
            var endlab = Compiler.NewTemp(true);

            var enterLabel = Compiler.RootLabel;
            ExitLabel = endlab;
            
            Type = TypeEnum.Bool;
            switch (operation)
            {
                case Operation.And:
                    Compiler.EmitCode($"br i1 {left.Identifier}, label %{rightlab}, label %{endlab}");
                    Compiler.EmitCode($"{rightlab}:");
                    Compiler.RootLabel = rightlab;
                    right.GenerateCode();
                    Compiler.EmitCode($"br label %{endlab}");
                    Compiler.EmitCode($"{endlab}:");
                    Identifier = Compiler.NewTemp();
                    Compiler.EmitCode($"{Identifier} = phi i1 [0, %{enterLabel}], " +
                                      $"[{right.Identifier}, %{right.ExitLabel ?? rightlab}]");
                    break;
                case Operation.Or:
                    Compiler.EmitCode($"br i1 {left.Identifier}, label %{endlab}, label %{rightlab}");
                    Compiler.EmitCode($"{rightlab}:");
                    Compiler.RootLabel = rightlab;
                    right.GenerateCode();
                    Compiler.EmitCode($"br label %{endlab}");
                    Compiler.EmitCode($"{endlab}:");
                    Identifier = Compiler.NewTemp();
                    Compiler.EmitCode($"{Identifier} = phi i1 [1, %{enterLabel}]," +
                                      $" [{right.Identifier}, %{right.ExitLabel ?? rightlab}]");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            Compiler.RootLabel = endlab;
            if (right.Type != TypeEnum.Bool)
            {
                Compiler.Error(right.Location, "expression is not of type bool");
            }
        }
        
        public enum Operation
        {
            And, Or
        }
    }
    
    public class RelationalExpression : SyntaxTree
    {
        private readonly SyntaxTree left;
        private readonly SyntaxTree right;
        private readonly Operation operation;

        public RelationalExpression(SyntaxTree left, SyntaxTree right, Operation operation, LexLocation location)
            : base(location)
        {
            this.left = left;
            this.right = right;
            this.operation = operation;
        }

        public override void GenerateCode()
        {
            left.GenerateCode();
            right.GenerateCode();
            
            var type = TypeEnum.Int.LlvmType();
            if (operation != Operation.Equal && operation != Operation.NotEqual)
            {
                if (left.Type == TypeEnum.Bool)
                {
                    Compiler.Error(left.Location, "expression cannot be bool");
                    return;
                }
                
                if (right.Type == TypeEnum.Bool)
                {
                    Compiler.Error(right.Location, "expression cannot be bool");
                    return;
                }
            }
            else
            {
                if (left.Type == TypeEnum.Bool && right.Type != TypeEnum.Bool)
                {
                    Compiler.Error(right.Location, "both expressions must be bool");
                    return;
                }
                
                if (left.Type != TypeEnum.Bool && right.Type == TypeEnum.Bool)
                {
                    Compiler.Error(left.Location, "both expressions must be bool");
                    return;
                }

                if (left.Type == TypeEnum.Bool && right.Type == TypeEnum.Bool)
                {
                    type = TypeEnum.Bool.LlvmType();
                }
            }
            
            Identifier = Compiler.NewTemp();
            Type = TypeEnum.Bool;

            string op;
            var com = "icmp";
            if (left.Type == TypeEnum.Double || right.Type == TypeEnum.Double)
            {
                type = TypeEnum.Double.LlvmType();
                com = "fcmp";
                
                switch (operation)
                {
                    case Operation.Equal:
                        op = "oeq";
                        break;
                    case Operation.NotEqual:
                        op = "one";
                        break;
                    case Operation.Greater:
                        op = "ogt";
                        break;
                    case Operation.GreaterEqual:
                        op = "oge";
                        break;
                    case Operation.Less:
                        op = "olt";
                        break;
                    case Operation.LessEqual:
                        op = "ole";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (left.Type == TypeEnum.Int)
                {
                    var tmp = Compiler.NewTemp();
                    Cast(left.Identifier, TypeEnum.Int, tmp, TypeEnum.Double);
                    left.Identifier = tmp;
                }
                
                if (right.Type == TypeEnum.Int)
                {
                    var tmp = Compiler.NewTemp();
                    Cast(right.Identifier, TypeEnum.Int, tmp, TypeEnum.Double);
                    right.Identifier = tmp;
                }
            }
            else
            {
                switch (operation)
                {
                    case Operation.Equal:
                        op = "eq";
                        break;
                    case Operation.NotEqual:
                        op = "ne";
                        break;
                    case Operation.Greater:
                        op = "sgt";
                        break;
                    case Operation.GreaterEqual:
                        op = "sge";
                        break;
                    case Operation.Less:
                        op = "slt";
                        break;
                    case Operation.LessEqual:
                        op = "sle";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            Compiler.EmitCode($"{Identifier} = {com} {op} {type} {left.Identifier}, {right.Identifier}");
        }
        
        public enum Operation
        {
            Equal, NotEqual, Greater, GreaterEqual, Less, LessEqual
        }
    }
    
    public class ArithmeticExpression : SyntaxTree
    {
        private readonly SyntaxTree left;
        private readonly SyntaxTree right;
        private readonly Operation operation;

        public ArithmeticExpression(SyntaxTree left, SyntaxTree right, Operation operation, LexLocation location) : base(location)
        {
            this.left = left;
            this.right = right;
            this.operation = operation;
        }

        public override void GenerateCode()
        {
            left.GenerateCode();
            right.GenerateCode();
            
            if (left.Type == TypeEnum.Bool)
            {
                Compiler.Error(left.Location, "expression cannot be bool");
                return;
            }
            if (right.Type == TypeEnum.Bool)
            {
                Compiler.Error(right.Location, "expression cannot be bool");
                return;
            }

            Identifier = Compiler.NewTemp();

            string op;
            if (left.Type == TypeEnum.Double || right.Type == TypeEnum.Double)
            {
                Type = TypeEnum.Double;

                switch (operation)
                {
                    case Operation.Addition:
                        op = "fadd";
                        break;
                    case Operation.Subtraction:
                        op = "fsub";
                        break;
                    case Operation.Multiplication:
                        op = "fmul";
                        break;
                    case Operation.Division:
                        op = "fdiv";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (left.Type == TypeEnum.Int)
                {
                    var tmp = Compiler.NewTemp();
                    Cast(left.Identifier, TypeEnum.Int, tmp, TypeEnum.Double);
                    left.Identifier = tmp;
                }
                
                if (right.Type == TypeEnum.Int)
                {
                    var tmp = Compiler.NewTemp();
                    Cast(right.Identifier, TypeEnum.Int, tmp, TypeEnum.Double);
                    right.Identifier = tmp;
                }
            }
            else
            {
                Type = TypeEnum.Int;
                
                switch (operation)
                {
                    case Operation.Addition:
                        op = "add";
                        break;
                    case Operation.Subtraction:
                        op = "sub";
                        break;
                    case Operation.Multiplication:
                        op = "mul";
                        break;
                    case Operation.Division:
                        op = "sdiv";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            Compiler.EmitCode($"{Identifier} = {op} {Type.LlvmType()} {left.Identifier}, {right.Identifier}");
        }
        
        public enum Operation
        {
            Addition, Subtraction, Multiplication, Division
        }
    }
    
    public class BinaryExpression : SyntaxTree
    {
        private readonly SyntaxTree left;
        private readonly SyntaxTree right;
        private readonly Operation operation;

        public BinaryExpression(SyntaxTree left, SyntaxTree right, Operation operation,LexLocation location) : base(location)
        {
            this.left = left;
            this.right = right;
            this.operation = operation;
        }

        public override void GenerateCode()
        {
            left.GenerateCode();
            right.GenerateCode();

            if (left.Type != TypeEnum.Int)
            {
                Compiler.Error(left.Location, "expression must be int");
                return;
            }
            
            if (right.Type != TypeEnum.Int)
            {
                Compiler.Error(right.Location, "expression must be int");
                return;
            }

            Identifier = Compiler.NewTemp();
            Type = TypeEnum.Int;

            string op;
            switch (operation)
            {
                case Operation.And:
                    op = "and";
                    break;
                case Operation.Or:
                    op = "or";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Compiler.EmitCode($"{Identifier} = {op} i32 {left.Identifier}, {right.Identifier}");
        }
        
        public enum Operation
        {
            And, Or
        }
    }
    
    public class UnaryExpression : SyntaxTree
    {
        private readonly SyntaxTree right;
        private readonly Operation operation;

        public UnaryExpression(SyntaxTree right, Operation operation, LexLocation location) : base(location)
        {
            this.right = right;
            this.operation = operation;
        }

        public override void GenerateCode()
        {
            right.GenerateCode();

            Identifier = Compiler.NewTemp();
            switch (operation)
            {
                case Operation.Minus:
                    UnaryMinus();
                    break;
                case Operation.Negate:
                    UnaryNegate();
                    break;
                case Operation.BitNegate:
                    UnaryBitNegate();
                    break;
                case Operation.CastInt:
                    UnaryCastInt();
                    break;
                case Operation.CastDouble:
                    UnaryCastDouble();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UnaryCastDouble()
        {
            Type = TypeEnum.Double;
            Identifier = Compiler.NewTemp();
            switch (right.Type)
            {
                case TypeEnum.Int:
                    Compiler.EmitCode($"{Identifier} = sitofp {right.Type.LlvmType()} {right.Identifier}" +
                                      $" to {Type.LlvmType()}");
                    break;
                case TypeEnum.Double:
                    Identifier = right.Identifier;
                    return;
                case TypeEnum.Bool:
                    Compiler.Error(right.Location, "cannot cast bool to double");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        private void UnaryCastInt()
        {
            Type = TypeEnum.Int;
            Identifier = Compiler.NewTemp();
            string op;
            switch (right.Type)
            {
                case TypeEnum.Int:
                    Identifier = right.Identifier;
                    return;
                case TypeEnum.Double:
                    op = "fptosi";
                    break;
                case TypeEnum.Bool:
                    op = "zext";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Compiler.EmitCode($"{Identifier} = {op} {right.Type.LlvmType()} {right.Identifier} to {Type.LlvmType()}");
        }

        private void UnaryNegate()
        {
            if (right.Type != TypeEnum.Bool)
            {
                Compiler.Error(right.Location, "expression must be bool");
                return;
            }
            
            Type = TypeEnum.Bool;
            Identifier = Compiler.NewTemp();
            Compiler.EmitCode($"{Identifier} = xor {TypeEnum.Bool.LlvmType()} {right.Identifier}, 1");
        }

        private void UnaryBitNegate()
        {
            if (right.Type != TypeEnum.Int)
            {
                Compiler.Error(right.Location, "expression must be int");
                return;
            }
            
            Type = TypeEnum.Int;
            Identifier = Compiler.NewTemp();
            Compiler.EmitCode($"{Identifier} = xor {TypeEnum.Int.LlvmType()} {right.Identifier}, -1");
        }

        private void UnaryMinus()
        {
            if (right.Type == TypeEnum.Bool)
            {
                Compiler.Error(right.Location, "expression cannot be bool");
                return;
            }

            Type = right.Type;
            Identifier = Compiler.NewTemp();
            Compiler.EmitCode(right.Type == TypeEnum.Double
                                  ? $"{Identifier} = fneg {TypeEnum.Double.LlvmType()} {right.Identifier}"
                                  : $"{Identifier} = sub {TypeEnum.Int.LlvmType()} 0, {right.Identifier}");
        }

        public enum Operation
        {
            Minus, Negate, BitNegate, CastInt, CastDouble
        }
    }
}
