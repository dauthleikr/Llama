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

        public long AddFunction(FunctionImplementation function, IEnumerable<FunctionDeclaration> declarations)
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
            if (function.Declaration.Identifier.RawText == "main")
                CompileEntryPointPreCode();
            _context.CompileStatement(function.Body.StatementAsBlock(), _codeGen, storageManager, scope);
            _codeGen.InsertCode(_context.AddressLinker, prologuePosition, gen => storageManager.CreatePrologue(gen));
            storageManager.CreateEpilogue(_codeGen);
            _codeGen.Ret();
            return prologuePosition;
        }

        private void CompileEntryPointPreCode()
        {
            _codeGen.CallRelative(Constants.DummyOffsetInt);
            _context.AddressLinker.FixIATEntryOffset(_codeGen.StreamPosition, "kernel32.dll", "GetProcessHeap");
            _codeGen.MovToDereferenced4(Constants.DummyOffsetInt, Register64.RAX);
            _context.AddressLinker.FixDataOffset(_codeGen.StreamPosition, Constants.HeapHandleIdentifier);
        }

        public ReadOnlySpan<byte> Finish() => _codeGen.GetDataSpan();
    }
}