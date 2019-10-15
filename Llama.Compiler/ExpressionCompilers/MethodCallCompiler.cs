namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using System.Linq;
    using Extensions;
    using Parser.Lexer;
    using Parser.Nodes;
    using spit;
    using Type = Parser.Nodes.Type;

    internal class MethodCallCompiler : ICompileExpressions<MethodCallExpression>
    {
        private readonly (Register64 intRegister, XmmRegister floatRegister)[] _parameterRegisters =
        {
            (Register64.RCX, XmmRegister.XMM0),
            (Register64.RDX, XmmRegister.XMM1),
            (Register64.R8, XmmRegister.XMM2),
            (Register64.R9, XmmRegister.XMM3)
        };

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

            Type[] parameterTypes;
            FunctionDeclaration knownDeclaration = null;
            if (expression.Expression is AtomicExpression atomicExpression && atomicExpression.Token.Kind == TokenKind.Identifier)
            {
                knownDeclaration = scope.GetFunctionDeclaration(atomicExpression.Token.RawText);
                parameterTypes = knownDeclaration.Parameters.Select(par => par.ParameterType).ToArray();
            }
            else
                parameterTypes = new Type[expression.Parameters.Length];

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

                if (parameterTypes[i] == null)
                    parameterTypes[i] = parameterResult.ValueType;

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
                    var stackReference = new ExpressionResult(
                        parameterResult.ValueType,
                        Register64.RSP,
                        scope.GetCalleeParameterOffset(i),
                        Segment.SS
                    );
                    stackReference.GenerateAssign(tempVolatile, codeGen, addressFixer);
                }
            }

            var functionPtr = context.CompileExpression(expression.Expression, codeGen, storageManager, target, scope);
            if (!functionPtr.ValueType.IsIntegerRegisterType())
                throw new TypeMismatchException(Constants.FunctionPointerType.ToString(), functionPtr.ValueType.ToString());

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

            for (var i = 0; i < _parameterRegisters.Length; i++)
            {
                var (intRegister, floatRegister) = _parameterRegisters[i];
                var register = parameterTypes[i].MakeRegisterWithCorrectSize(intRegister, floatRegister);
                parameterStorages[i].AsExpressionResult(parameterTypes[i]).GenerateMoveTo(register, parameterTypes[i], codeGen, addressFixer);
            }

            switch (functionPtr.Kind)
            {
                case ExpressionResult.ResultKind.Offset:
                    codeGen.CallRelative(Constants.DummyOffsetInt);
                    functionPtr.OffsetFixup(addressFixer, codeGen);
                    break;
                case ExpressionResult.ResultKind.Value:
                    codeGen.Call(functionPtr.Value.AsR64());
                    break;
                default:
                    throw new NotImplementedException();
            }

            var returnType = knownDeclaration?.ReturnType ?? Constants.LongType; // todo: better alternative if not known
            return new ExpressionResult(returnType, returnType.MakeRegisterWithCorrectSize(Register64.RAX, XmmRegister.XMM0));
        }
    }
}