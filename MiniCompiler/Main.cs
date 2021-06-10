using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using QUT.Gppg;

namespace MiniCompiler
{ 
    public static class Compiler
    {
        public static int Errors = 0;
        public static readonly Dictionary<string, TypeEnum> Symbols = new Dictionary<string, TypeEnum>();
        public static readonly List<string> StringLiterals = new List<string>();
        public static SyntaxTree Program = null;

        private static StreamWriter sw;
        private static int temps = 0;

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
            parser.Parse();

            if (Errors == 0 && Program != null)
            {   
                GenProlog();
                
                Program.GenerateCode();
                sw.Close();
                source.Close();
            }

            if (Errors != 0)
            {
                Console.WriteLine($"\n  {Errors} errors detected\n");
                File.Delete(file + ".ll");
                return 2;
            }
            return 0;
        }

        public static string NewTemp()
        {
            return $"__{++temps}";
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

            foreach (var literal in StringLiterals)
            {
                EmitCode($"@__{literal} = constant [{literal.Length + 1} x i8] c\"{literal}\\00\"");
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
            return;
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
            
            var truelab = Compiler.NewTemp();
            var falselab = Compiler.NewTemp();
            var endlab = Compiler.NewTemp();
            
            condition.GenerateCode();
            Compiler.EmitCode($"br i1 %{condition?.Identifier}, label %{truelab}, label %{falselab}");
            Compiler.EmitCode($"{truelab}:");
            thenInstruction?.GenerateCode();
            if (elseInstruction != null)
            {
                Compiler.EmitCode($"br label %{endlab}");
                Compiler.EmitCode($"{falselab}:");
                elseInstruction.GenerateCode();
            }
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
            
            var startlab = Compiler.NewTemp();
            var innerlab = Compiler.NewTemp();
            var endlab = Compiler.NewTemp();
            
            Compiler.EmitCode($"br label %{startlab}");
            Compiler.EmitCode($"{startlab}:");
            condition.GenerateCode();
            Compiler.EmitCode($"br i1 %{condition?.Identifier}, label %{innerlab}, label %{endlab}");
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
}
