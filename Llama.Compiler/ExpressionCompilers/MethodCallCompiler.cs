namespace Llama.Compiler.ExpressionCompilers
{
    using System.Collections.Generic;
    using Extensions;
    using Parser.Nodes;
    using spit;

    internal class MethodCallCompiler : ICompileExpressions<MethodCallExpression>
    {
        public ExpressionResult Compile(
            MethodCallExpression expression,
            PreferredRegister target,
            CodeGen codeGen,
            StorageManager storageManager,
            IScopeContext scope,
            IAddressFixer addressFixer,
            ICompilationContext context
        )
        {
            var parameterStorages = new Storage[4];
            var parameterTypes = new Type();
            for (var i = 0; i < expression.Parameters.Length; i++)
            {
                var parameter = expression.Parameters[i];
                var parameterResult = context.CompileExpression(
                    parameter,
                    codeGen,
                    storageManager,
                    new PreferredRegister(Register64.RAX, XmmRegister.XMM0), // todo: improve by peeking the StorageManager
                    scope
                );
                if (i < 4) // First 4 parameters are passed as RCX, RDX, R8, R9
                {
                    var storage = storageManager.Allocate(parameterResult.ValueType.IsIntegerRegisterType());
                    storage.Store(parameterResult, codeGen, addressFixer);
                    parameterStorages[i] = storage;
                }
                else // Remaining parameters are passed on the stack
                {
                    var tempVolatile = parameterResult.GetUnoccupiedVolatile(parameterResult.ValueType.IsIntegerRegisterType());
                    parameterResult.GenerateMoveTo(tempVolatile, codeGen, addressFixer);
                    var stackReference = new ExpressionResult(parameterResult.ValueType, Register64.RSP, scope.GetCalleeParameterOffset(i), Segment.SS);
                    stackReference.GenerateAssign(tempVolatile, codeGen, addressFixer);
                }

            }

            var functionPtr = context.CompileExpression(expression.Expression, codeGen, storageManager, target, scope);
            if (functionPtr.Kind != ExpressionResult.ResultKind.Offset &&
                (functionPtr.Kind != ExpressionResult.ResultKind.Value ||
                 functionPtr.Value.IsSameRegister(Register64.RCX) ||
                 functionPtr.Value.IsSameRegister(Register64.RDX) ||
                 functionPtr.Value.IsSameRegister(Register64.R8) ||
                 functionPtr.Value.IsSameRegister(Register64.R9)))
            {
                functionPtr.GenerateMoveTo(Register64.RAX, codeGen, addressFixer);
                functionPtr = new ExpressionResult(functionPtr.ValueType, Register64.RAX);
            }


            // todo: move parameters from parameterStorages into RCX, RDX, ...
            // todo: we need to type somehow to perform upgrades - for that we'd need the signature of the callee ...
            // todo: cant just use the types from the parameters up there, it would not perform an upgrade ...
            //parameterStorages[0].AsExpressionResult()
        }
    }
}