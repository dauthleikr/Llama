namespace Llama.Compiler.ExpressionCompilers
{
    using Parser.Nodes;
    using spit;

    internal class ArrayAllocationCompiler : ICompileExpressions<ArrayAllocationExpression>
    {
        public void Compile(
            ArrayAllocationExpression expression,
            Register target,
            CodeGen codeGen,
            IFunctionContext function,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            context.CompileExpression(expression.Count, codeGen, Register64.RAX, function);

            // Array allocation compiles to function call of HeapAlloc (kernel32)
            codeGen.MovFromDereferenced(Register64.RCX, Constants.DummyOffsetInt);
            addressFixer.FixConstantDataOffset(codeGen, Constants.HeapHandle);
            codeGen.Xor(Register64.RDX, Register64.RDX);
            codeGen.Add(Register64.RDX, (sbyte)(0x8 + 0x4)); // HEAP_ZERO_MEMORY + HEAP_GENERATE_EXCEPTIONS
            codeGen.Imul(Register64.R8, Register64.RAX, (sbyte)expression.Type.SizeOf()); // todo: rework if adding structs
            codeGen.CallRelative(Constants.DummyOffsetInt);
            addressFixer.FixIATEntryOffset(codeGen, "kernel32.dll", "HeapAlloc");
        }
    }
}