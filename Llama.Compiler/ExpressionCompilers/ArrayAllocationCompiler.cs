namespace Llama.Compiler.ExpressionCompilers
{
    using Parser.Nodes;
    using spit;

    internal class ArrayAllocationCompiler : ICompileExpressions<ArrayAllocationExpression>
    {
        public Type Compile(
            ArrayAllocationExpression expression,
            Register target,
            CodeGen codeGen,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var indexType = context.CompileExpression(expression.Count, codeGen, Register64.RAX, scope);
            Constants.LongType.AssertCanAssign(indexType);

            // if the original value is less than 4 byte, the asm instructions do not guarantee that the upper part of the target register is cleared
            if (indexType.SizeOf() < 4)
                codeGen.And(Register64.RAX, (1 << (8 * indexType.SizeOf())) - 1);

            // Array allocation compiles to function call of HeapAlloc (kernel32)
            codeGen.MovFromDereferenced(Register64.RCX, Constants.DummyOffsetInt);
            addressFixer.FixDataOffset(codeGen, Constants.HeapHandleIdentifier); // Parameter 1: DefaultHeapHandle

            codeGen.Xor(Register64.RDX, Register64.RDX);
            codeGen.Add(Register64.RDX, (sbyte)(0x8 + 0x4)); // Parameter 2: HEAP_ZERO_MEMORY + HEAP_GENERATE_EXCEPTIONS
            // todo: rework if adding structs:
            codeGen.Imul(Register64.R8, Register64.RAX, (sbyte)expression.Type.SizeOf()); // Parameter 3: byte count 

            codeGen.CallRelative(Constants.DummyOffsetInt);
            addressFixer.FixIATEntryOffset(codeGen, "kernel32.dll", "HeapAlloc");
            return new Type(expression.Type, Type.WrappingType.ArrayOf);
        }
    }
}