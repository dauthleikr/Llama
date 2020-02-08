namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using Extensions;
    using Parser.Nodes;
    using spit;
    using Type = Parser.Nodes.Type;

    internal class ArrayAllocationCompiler : ICompileExpressions<ArrayAllocationExpression>
    {
        public ExpressionResult Compile(
            ArrayAllocationExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            ISymbolResolver scope,
            ILinkingInfo linkingInfo,
            ICompilationContext context
        )
        {
            var index = context.CompileExpression(expression.Count, codeGen, storageManager, new PreferredRegister(Register64.R8), scope);
            var indexType = index.ValueType;
            var valueTypeSize = (sbyte)expression.Type.SizeOf();
            Constants.LongType.AssertCanAssignImplicitly(indexType);

            // Array allocation compiles to function call of HeapAlloc (kernel32)
            index.GenerateMoveTo(Register64.R8, Constants.LongType, codeGen, linkingInfo); // Parameter 3: byte count
            if (valueTypeSize > 1)
                codeGen.Shl(Register64.R8, (sbyte)Math.Log(valueTypeSize, 2)); // 8 byte type -> multiply count by 8 by shifting left 3
            codeGen.Add(Register64.R8, (sbyte)8); // llama length value


            codeGen.MovFromDereferenced4(Register64.RCX, Constants.DummyOffsetInt);
            linkingInfo.FixDataOffset(codeGen.StreamPosition, Constants.HeapHandleIdentifier); // Parameter 1: DefaultHeapHandle

            codeGen.Xor(Register64.RDX, Register64.RDX);
            codeGen.Add(Register64.RDX, (sbyte)(0x8 + 0x4)); // Parameter 2: HEAP_ZERO_MEMORY + HEAP_GENERATE_EXCEPTIONS

            codeGen.CallDereferenced4(Constants.DummyOffsetInt);
            linkingInfo.FixIATEntryOffset(codeGen.StreamPosition, "kernel32.dll", "HeapAlloc");

            return new ExpressionResult(new Type(expression.Type, Type.WrappingType.ArrayOf), Register64.RAX);
        }
    }
}