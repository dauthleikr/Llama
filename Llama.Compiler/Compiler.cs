namespace Llama.Compiler
{
    using System;
    using System.IO;
    using System.Linq;
    using Parser.Nodes;
    using spit;

    public class Compiler
    {
        private readonly CodeGen _codeGen;
        private readonly ICompilationContext _context;
        private readonly MemoryStream _rawStream = new MemoryStream();

        public Compiler(ICompilationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _codeGen = new CodeGen(_rawStream);
        }

        public void AddFunction(FunctionImplementation function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));

            _codeGen.Write(0xCC);
            _codeGen.Write(
                Enumerable.Repeat((byte)0xCC, (int)(_codeGen.StreamPosition % 16)).ToArray()
            ); // 16-byte align function with int3 breakpoints

            var scope = FunctionScope.FromBlock(function);
            var storageManager = new StorageManager(scope);

            storageManager.CreatePrologue(_codeGen);
            _context.CompileStatement(function.Body.StatementAsBlock(), _codeGen, storageManager, scope);
            storageManager.CreateEpilogue(_codeGen);
            _codeGen.Ret();
        }

        public byte[] Finish() => _rawStream.ToArray();
    }
}