namespace Llama.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Parser.Nodes;
    using spit;

    public class Compiler
    {
        private readonly CodeGen _codeGen = new CodeGen();
        private readonly ICompilationContext _context;

        public Compiler(ICompilationContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public void AddFunction(FunctionImplementation function, IEnumerable<FunctionDeclaration> declarations)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));
            if (declarations == null)
                throw new ArgumentNullException(nameof(declarations));

            _codeGen.Write(0xCC);
            _codeGen.Write(
                Enumerable.Repeat((byte)0xCC, (int)(_codeGen.StreamPosition % 16)).ToArray()
            ); // 16-byte align function with int3 breakpoints

            var scope = FunctionScope.FromBlock(function, declarations);
            var storageManager = new StorageManager(scope);

            var prologuePosition = _codeGen.StreamPosition;
            _context.CompileStatement(function.Body.StatementAsBlock(), _codeGen, storageManager, scope);
            _codeGen.InsertCode(_context.AddressLinker, prologuePosition, gen => storageManager.CreatePrologue(gen));
            storageManager.CreateEpilogue(_codeGen);
            _codeGen.Ret();
        }

        public ReadOnlySpan<byte> Finish() => _codeGen.GetDataSpan();
    }
}