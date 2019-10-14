namespace Llama.Compiler.ExpressionCompilers
{
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class ArrayAccessCompiler : ICompileExpressions<ArrayAccessExpression>
    {
        public ExpressionResult Compile(
            ArrayAccessExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var arrayTemp = storageManager.Allocate(true);
            var array = context.CompileExpression(expression.Array, codeGen, storageManager, arrayTemp.Register, scope);
            var arrayType = array.ValueType;
            arrayTemp.Store(array, codeGen, addressFixer);

            const Register64 structOffsetRegister = Register64.RCX;
            const Register64 arrayRegister = Register64.RAX;
            var arrayIndex = context.CompileExpression(expression.Index, codeGen, storageManager, new PreferredRegister(structOffsetRegister), scope);
            Constants.LongType.AssertCanAssign(arrayIndex.ValueType);

            arrayIndex.GenerateMoveTo(structOffsetRegister, codeGen, addressFixer);
            arrayTemp.AsExpressionResult(arrayType).GenerateMoveTo(arrayRegister, codeGen, addressFixer);
            storageManager.Release(arrayTemp);

            var itemType = arrayType.Child;
            if (arrayType.ChildRelation == Type.WrappingType.PointerOf)
                return new ExpressionResult(itemType, arrayRegister, structOffsetRegister, (byte)itemType.SizeOf());
            if (arrayType.ChildRelation == Type.WrappingType.ArrayOf)
                return new ExpressionResult(itemType, arrayRegister, structOffsetRegister, (byte)itemType.SizeOf(), 8);

            throw new TypeMismatchException("Array or pointer", arrayType.ToString());
        }
    }
}