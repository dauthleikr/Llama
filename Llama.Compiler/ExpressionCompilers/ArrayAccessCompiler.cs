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
            ICompilationContext context
        )
        {
            var arrayTemp = context.Storage.Allocate(true);
            var array = context.CompileExpression(expression.Array, arrayTemp.IsRegister ? arrayTemp.Register : Register64.RAX);
            var arrayType = array.ValueType;
            arrayTemp.Store(array, context.Generator, context.Linking);

            const Register64 structOffsetRegister = Register64.RCX;
            const Register64 arrayRegister = Register64.RAX;
            var arrayIndex = context.CompileExpression(expression.Index, new PreferredRegister(structOffsetRegister));
            Constants.LongType.AssertCanAssignImplicitly(arrayIndex.ValueType);

            arrayIndex.GenerateMoveTo(structOffsetRegister, Constants.LongType, context.Generator, context.Linking);
            arrayTemp.AsExpressionResult(arrayType).GenerateMoveTo(arrayRegister, context.Generator, context.Linking);
            context.Storage.Release(arrayTemp);

            if (arrayType == Constants.CstrType)
                return new ExpressionResult(Constants.SbyteType, arrayRegister, structOffsetRegister, 1);

            var itemType = arrayType.Child;
            if (arrayType.ChildRelation == Type.WrappingType.PointerOf)
                return new ExpressionResult(itemType, arrayRegister, structOffsetRegister, (byte)itemType.SizeOf());
            if (arrayType.ChildRelation == Type.WrappingType.ArrayOf)
                return new ExpressionResult(itemType, arrayRegister, structOffsetRegister, (byte)itemType.SizeOf(), 8);

            throw new TypeMismatchException("Array or pointer", arrayType.ToString());
        }
    }
}