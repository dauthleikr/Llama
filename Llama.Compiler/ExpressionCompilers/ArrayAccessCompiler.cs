namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using Parser.Nodes;
    using spit;
    using Type = Parser.Nodes.Type;

    internal class ArrayAccessCompiler : ICompileExpressions<ArrayAccessExpression>
    {
        public Type Compile(
            ArrayAccessExpression expression,
            Register target,
            CodeGen codeGen,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var arrayRegister = new Register(Register64.RAX);
            var indexRegister = new Register(Register64.RCX);
            var arrayType = context.CompileExpression(expression.Array, codeGen, arrayRegister, scope);
            var arrayIndexType = context.CompileExpression(expression.Index, codeGen, indexRegister, scope);
            Constants.LongType.AssertCanAssign(arrayIndexType);

            var itemType = arrayType.Child;
            if (arrayType.ChildRelation == Type.WrappingType.PointerOf)
                CompilePointerAccess(target, codeGen, itemType, arrayRegister, indexRegister, arrayType);
            else if (arrayType.ChildRelation == Type.WrappingType.ArrayOf)
                CompilePointerAccess(target, codeGen, itemType, arrayRegister, indexRegister, arrayType, 8);
            else
                throw new TypeMismatchException("Array or pointer", arrayType.ToString());

            return itemType;
        }

        private static Type CompilePointerAccess(
            Register target,
            CodeGen codeGen,
            Type itemType,
            Register arrayRegister,
            Register indexRegister,
            Type arrayType,
            sbyte offset = 0
        )
        {
            if (itemType.IsIntegerType() && target.CanUseIntegerRegister)
                codeGen.MovFromDereferenced2(
                    target.IntegerRegister,
                    arrayRegister.IntegerRegister,
                    indexRegister.IntegerRegister,
                    (byte)arrayType.Child.SizeOf(),
                    offset
                );
            else if (itemType == Constants.DoubleType && target.CanUseFloatRegister)
                codeGen.MovsdFromDereferenced2(
                    target.FloatRegister,
                    arrayRegister.IntegerRegister,
                    indexRegister.IntegerRegister,
                    (byte)arrayType.Child.SizeOf(),
                    offset
                );
            else if (itemType == Constants.FloatType && target.CanUseFloatRegister)
                codeGen.MovssFromDereferenced2(
                    target.FloatRegister,
                    arrayRegister.IntegerRegister,
                    indexRegister.IntegerRegister,
                    (byte)arrayType.Child.SizeOf(),
                    offset
                );
            else
                throw new NotImplementedException(
                    $"{nameof(ArrayAccessCompiler)}: I do not know how to compile this type: {itemType} with target register: {target}"
                );
            return itemType;
        }
    }
}