using System;
using System.IO;
using System.Collections.Generic;

namespace MiniCompiler
{
    public enum TypeEnum {
        Int, Double, Bool, String
    }

    public static class TypeEnumExtension
    {
        public static string LLVMType(this TypeEnum type)
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
    
    public struct Exp
    {
        public string Identifier;
        public TypeEnum Type;

        public Exp(string identifier, TypeEnum type)
        {
            Identifier = identifier;
            Type = type;
        }
    }

    public class Compiler
    {
        public static int Errors = 0;
        public static Dictionary<string, TypeEnum> Identifiers = new Dictionary<string, TypeEnum>();

        private static StreamWriter sw;
        private static int temps = 0;

        // arg[0] określa plik źródłowy
        // pozostałe argumenty są ignorowane
        public static int Main(string[] args)
        {
            string file;
            FileStream source;
            Console.WriteLine("\nMini Language Compiler");
            if (args.Length >= 1)
                file = args[0];
            else
            {
                Console.Write("\nsource file:  ");
                file = Console.ReadLine();
            }
            try
            {
                var sr = new StreamReader(file);
                string str = sr.ReadToEnd();
                sr.Close();
                source = new FileStream(file, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                return 1;
            }
            Scanner scanner = new Scanner(source);
            Parser parser = new Parser(scanner);
            Console.WriteLine();
            sw = new StreamWriter(file + ".ll");
            GenProlog();
            parser.Parse();
            GenEpilog();
            sw.Close();
            source.Close();
            if (Errors == 0)
                Console.WriteLine("  compilation successful\n");
            else
            {
                Console.WriteLine($"\n  {Errors} errors detected\n");
                File.Delete(file + ".ll");
            }
            return Errors == 0 ? 0 : 2;
        }

        public static string NewTemp()
        {
            return $"%__{++temps}";
        }

        public static void EmitCode(string instr = null)
        {
            sw.WriteLine(instr);
        }

        public static void EmitCode(string instr, params object[] args)
        {
            sw.WriteLine(instr, args);
        }

        private static void GenProlog()
        {
            EmitCode("; prolog");
            EmitCode("@i32 = constant [4 x i8] c\"%d\\0A\\00\"");
            EmitCode("@hex = constant [6 x i8] c\"0X%X\\0A\\00\"");
            EmitCode("@str = constant [4 x i8] c\"%s\\0A\\00\"");
            EmitCode("@double = constant [5 x i8] c\"%lf\\0A\\00\"");
            EmitCode("@true = constant [6 x i8] c\"True\\0A\\00\"");
            EmitCode("@false = constant [7 x i8] c\"False\\0A\\00\"");
            EmitCode("declare i32 @printf(i8*, ...)");
            EmitCode("define i32 @main()");
            EmitCode("{");
            EmitCode();
        }

        private static void GenEpilog()
        {
            EmitCode();
            EmitCode("ret i32 0");
            EmitCode("}");
        }

    }
}
