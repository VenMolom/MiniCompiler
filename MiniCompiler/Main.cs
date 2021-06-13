using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security;
using System.Security.Cryptography;
using QUT.Gppg;

namespace MiniCompiler
{ 
    public static class Compiler
    {
        public static readonly Dictionary<string, TypeEnum> Symbols = new Dictionary<string, TypeEnum>();
        public static readonly Dictionary<string, (string ident, int length)> StringLiterals =
            new Dictionary<string, (string, int)>();
        public static SyntaxTree Program = null;

        private static StreamWriter sw;
        private static int temps = 0;
        private static int errors = 0;

        public static int Main(string[] args)
        {
            Console.WriteLine("\nMini Language Compiler");

            string file;
            FileStream source;
            if (args.Length >= 1)
            {
                file = args[0];
            }
            else
            {
                Console.Write("\nsource file:  ");
                file = Console.ReadLine();
            }
            
            try
            {
                source = new FileStream(file, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                return 1;
            }
            
            Scanner scanner = new Scanner(source);
            Parser parser = new Parser(scanner);
            
            sw = new StreamWriter(file + ".ll");

            try
            {
                parser.Parse();
            }
            catch
            {
                sw.Close();
                source.Close();
                Console.WriteLine($"\n{errors} errors detected\n");
                File.Delete(file + ".ll");
                return 2;
            }

            if (errors == 0 && Program != null)
            {   
                GenProlog();
                Program.GenerateCode();
                sw.Close();
                source.Close();
            }

            if (errors != 0)
            {
                sw.Close();
                source.Close();
                Console.WriteLine($"\n{errors} errors detected\n");
                File.Delete(file + ".ll");
                return 2;
            }
            return 0;
        }

        public static string NewTemp(bool label = false)
        {
            var prefix = label ? string.Empty : "%";
            return $"{prefix}__{++temps}";
        }

        public static void EmitCode(string instr = null)
        {
            sw.WriteLine(instr);
        }

        public static void Error(LexLocation location, string text = "syntax error")
        {
            Console.WriteLine($"Error ({location.StartLine},{location.StartColumn}): {text}");
            errors++;
        }

        public static void AddLiteral(string literal, int lengthModif)
        {
            StringLiterals.Add(literal, (NewTemp(true), literal.Length + lengthModif + 1));
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

        protected SyntaxTree(LexLocation location)
        {
            Location = location;
        }

        public abstract void GenerateCode();
        
        protected void Load(string source)
        {
            Compiler.EmitCode($"{Identifier} = load {Type.LlvmType()}, {Type.LlvmType()}* {source}");
        }

        protected void Store(string source, string target)
        {
            Compiler.EmitCode($"store {Type.LlvmType()} {source}, {Type.LlvmType()}* {target}");
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
            Compiler.EmitCode("define i32 @main()");
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
    
            Compiler.Symbols.Add(identifier, type);
            Compiler.EmitCode($"{identifier} = alloca {type.LlvmType()}");
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
            if (condition.Type != TypeEnum.Bool)
            {
                Compiler.Error(condition.Location, "condition needs to be of type bool");
                return;
            }
            
            var truelab = Compiler.NewTemp(true);
            var falselab = Compiler.NewTemp(true);
            var endlab = Compiler.NewTemp(true);
            
            condition.GenerateCode();
            Compiler.EmitCode($"br i1 {condition?.Identifier}, label %{truelab}, label %{falselab}");
            Compiler.EmitCode($"{truelab}:");
            thenInstruction?.GenerateCode();
            Compiler.EmitCode($"br label %{endlab}");
            Compiler.EmitCode($"{falselab}:");
            elseInstruction?.GenerateCode();
            Compiler.EmitCode($"br label %{endlab}");
            Compiler.EmitCode($"{endlab}:");
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
            if (condition.Type != TypeEnum.Bool)
            {
                Compiler.Error(condition.Location, "condition needs to be of type bool");
                return;
            }
            
            var startlab = Compiler.NewTemp(true);
            var innerlab = Compiler.NewTemp(true);
            var endlab = Compiler.NewTemp(true);
            
            Compiler.EmitCode($"br label %{startlab}");
            Compiler.EmitCode($"{startlab}:");
            condition.GenerateCode();
            Compiler.EmitCode($"br i1 {condition?.Identifier}, label %{innerlab}, label %{endlab}");
            Compiler.EmitCode($"{innerlab}:");
            instruction?.GenerateCode();
            Compiler.EmitCode($"br label %{startlab}");
            Compiler.EmitCode($"{endlab}:");
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
            expression?.GenerateCode();

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
            
            switch (flag)
            {
                case Flag.None:
                    if (expression.Type == TypeEnum.Bool)
                    {
                        PrintBool();
                        return;
                    }

                    var length = expression.Type == TypeEnum.Double ? 4 : 3;
                    Print(length);
                    break;
                case Flag.Hex:
                    if (expression.Type != TypeEnum.Int)
                    {
                        Compiler.Error(expression.Location, "variable must be int to be printed as hex");
                        return;
                    }

                    Print(6, "hex");
                    break;
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

            var type = Compiler.Symbols[identifier.Identifier];
            if (hex)
            {
                if (type != TypeEnum.Int)
                {
                    Compiler.Error(Location, "variable must be int to be read as hex");
                    return;
                }
                
                Read(TypeEnum.Int.LlvmType(), 3, "hexread");
            }
            
            if (type == TypeEnum.Bool)
            {
                Compiler.Error(Location, "cannot read into bool");
                return;
            }
            
            var length = type == TypeEnum.Double ? 4 : 3;
            Read(type.LlvmType(), length);
        }
        
        private void Read(string type, int length, string format = null)
        {
            Compiler.EmitCode($"call i32 (i8*, ...) @printf(i8* bitcast ([{length} x i8]* " +
                              $"@{format ?? type} to i8*)" +
                              $", {type}* {identifier.Identifier})");
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

            Type = Compiler.Symbols[identifier];
            Identifier = Compiler.NewTemp();
            Load(identifier);
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
            expression?.GenerateCode();
            
            if (!Compiler.Symbols.ContainsKey(identifier))
            {
                Compiler.Error(Location, "undeclared variable");
                return;
            }

            Type = Compiler.Symbols[identifier];
            if (Type != expression?.Type)
            {
                Compiler.Error(Location, "type mismatch");
                return;
            }

            Store(expression?.Identifier, identifier);
            Identifier = Compiler.NewTemp();
            Load(identifier);
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
}
