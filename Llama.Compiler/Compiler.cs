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
        public Span<byte> Code => Generator.GetBufferSpan();
        public readonly CodeGen Generator = new CodeGen();
        public readonly ILinkingInfo LinkingInfo;

        private readonly ICompilerStore _nodeCompilers;
        private readonly IFactory<ILinkingInfo> _linkingInfoFactory;
        private readonly IEnumerable<FunctionImport> _imports;
        private readonly IEnumerable<FunctionDeclaration> _declarations;

        public Compiler(ICompilerStore nodeCompilers, IFactory<ILinkingInfo> linkingInfoFactory, IEnumerable<FunctionImport> imports, IEnumerable<FunctionDeclaration> declarations)
        {
            _nodeCompilers = nodeCompilers ?? throw new ArgumentNullException(nameof(nodeCompilers));
            _linkingInfoFactory = linkingInfoFactory ?? throw new ArgumentNullException(nameof(linkingInfoFactory));
            _imports = imports ?? throw new ArgumentNullException(nameof(imports));
            _declarations = declarations ?? throw new ArgumentNullException(nameof(declarations));

            LinkingInfo = _linkingInfoFactory.Create();
        }

        public long AddFunction(FunctionImplementation function)
        {
            // 0. Setting up scope and storage for function
            var scope = FunctionSymbolResolver.FromBlock(function, _imports, _declarations);
            var storageManager = new StorageManager(scope);
            var context = new CompilationContext(_nodeCompilers, _linkingInfoFactory, scope, storageManager);

            // 1. Compiling the full function (incl. prologue and epilogue) with the given context
            CompileFunction(function, context);

            // 2. Create function padding in the global "context" (16-byte align function with int3 breakpoints)
            Generator.Write(0xCC);
            Generator.Write(
                Enumerable.Repeat((byte)0xCC, (int)(16 - Generator.StreamPosition % 16)).ToArray()
            );

            // 3. Copy code and linking info from the function context over to the global "context" 
            var functionStart = Generator.StreamPosition;
            context.Linking.CopyTo(LinkingInfo, Generator.StreamPosition);
            Generator.Write(context.Generator.GetBufferSpan());

            // 4. Add linking info for the newly compiled function, so that calls to it can get resolved correctly
            LinkingInfo.ResolveFunctionFixes(function.Declaration.Identifier.RawText, functionStart);
            return functionStart;
        }

        private void CompileFunction(FunctionImplementation function, ICompilationContext context)
        {
            foreach (var parameter in function.Declaration.Parameters)
                context.Symbols.DefineLocal(parameter.ParameterIdentifier.RawText, parameter.ParameterType);

            if (function.Declaration.Identifier.RawText == "Main")
            {
                context.Generator.CallDereferenced4(Constants.DummyOffsetInt);
                context.Linking.FixIATEntryOffset(context.Generator.StreamPosition, "kernel32.dll", "GetProcessHeap");
                context.Generator.MovToDereferenced4(Constants.DummyOffsetInt, Register64.RAX);
                context.Linking.FixDataOffset(context.Generator.StreamPosition, Constants.HeapHandleIdentifier);
            }

            context.CompileStatement(function.Body.StatementAsBlock());

            context.Generator.InsertCode(context.Linking, 0, gen => context.Storage.CreatePrologue(gen, function.Declaration));
            context.Linking.ResolveFunctionEpilogueFixes(function.Declaration.Identifier.RawText, context.Generator.StreamPosition);
            context.Storage.CreateEpilogue(context.Generator);
            context.Generator.Ret();
        }
    }
}