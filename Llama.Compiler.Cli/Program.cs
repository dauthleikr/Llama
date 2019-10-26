namespace Llama.Compiler.Cli
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Linker;
    using Parser;
    using Parser.Lexer;
    using Parser.Nodes;
    using PE.Builder.PE32Plus;

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1 || args.Any(arg => !File.Exists(arg)))
                Console.WriteLine($"Usage {Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location)} codefile.llama [, codefile2.llama, ...]");

            var time = Stopwatch.StartNew();
            var sourceFilePath = args[0];
            var source = string.Join("\n", args.Select(File.ReadAllText));
            var lexer = LexerBuilder.BuildLlamaLexer();
            var parser = new ParseContext(LlamaParseStore.Instance, lexer, source);
            var document = parser.ReadNode<LlamaDocument>();

            Console.WriteLine($"Parsing time: {time.Elapsed.TotalMilliseconds:F2} ms");
            time.Restart();

            var compilationContext = new CompilationContext(new LlamaCompilerStore(), new LinkerFactory());
            var compiler = new Compiler(compilationContext);

            foreach (var functionImplementation in document.Functions)
                compiler.AddFunction(functionImplementation, document.Imports, document.Functions.Select(fun => fun.Declaration));
            var codeBlob = compiler.Finish();
            var linker = (Linker)compilationContext.AddressLinker;

            Console.WriteLine($"Compilation time: {time.Elapsed.TotalMilliseconds:F2} ms");
            time.Restart();

            var pe32PlusExeBuilder = new ExecutableBuilder();
            linker.LinkPreBuild(pe32PlusExeBuilder);

            var mainOffset = linker.GetCodeOffsetOfKnownFunction("Main");
            if (mainOffset < 0)
            {
                Console.WriteLine("No main function");
                return;
            }

            var peResult = pe32PlusExeBuilder.Build((uint)codeBlob.Length, (uint)mainOffset);
            codeBlob.CopyTo(peResult.GetCodeSectionBuffer());
            linker.LinkPostBuild(peResult);

            Console.WriteLine($"Linking time: {time.Elapsed.TotalMilliseconds:F2} ms");
            time.Restart();

            using var fileStream = File.Open(Path.ChangeExtension(sourceFilePath, "exe"), FileMode.Create);
            peResult.Finish(fileStream);
        }
    }
}