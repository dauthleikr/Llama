namespace Llama.Compiler.Cli
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.PortableExecutable;
    using Linker;
    using Parser;
    using Parser.Lexer;
    using Parser.Nodes;
    using PE.Builder.PE32Plus;

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1 || !File.Exists(args[0]))
                Console.WriteLine($"Usage {Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location)} codefile.llama");

            var sourceFilePath = args[0];
            var source = File.ReadAllText(sourceFilePath);
            var lexer = LexerBuilder.BuildLlamaLexer();
            var parser = new ParseContext(new LlamaParseStore(), lexer, source);
            var document = parser.ReadNode<LlamaDocument>();

            var compilationContext = new CompilationContext(new LlamaCompilerStore(), new LinkerFactory());
            var compiler = new Compiler(compilationContext);

            var declarations = document.Functions.Select(fun => fun.Declaration).Concat(document.Imports.Select(imp => imp.Declaration)).ToArray();
            foreach (var functionImplementation in document.Functions)
                compiler.AddFunction(functionImplementation, declarations);
            var codeBlob = compiler.Finish();
            var linker = (Linker)compilationContext.AddressLinker;

            var pe32PlusExeBuilder = new ExecutableBuilder();
            linker.LinkPreBuild(pe32PlusExeBuilder);

            var mainOffset = linker.GetCodeOffsetOfKnownFunction("main");
            if (mainOffset < 0)
            {
                Console.WriteLine("No main function");
                return;
            }

            var peResult = pe32PlusExeBuilder.Build((uint)codeBlob.Length, (uint)mainOffset);
            linker.LinkPostBuild(peResult);
            codeBlob.CopyTo(peResult.GetCodeSectionBuffer());

            using var fileStream = File.Open(Path.ChangeExtension(sourceFilePath, "exe"), FileMode.Create);
            peResult.Finish(fileStream);
        }
    }
}