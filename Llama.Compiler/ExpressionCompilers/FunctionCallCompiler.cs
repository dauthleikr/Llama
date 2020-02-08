namespace Llama.Compiler.ExpressionCompilers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Parser.Lexer;
    using Parser.Nodes;
    using spit;
    using Type = Parser.Nodes.Type;

    internal class FunctionCallCompiler : ICompileExpressions<FunctionCallExpression>
    {
        private readonly (Register64 intRegister, XmmRegister floatRegister)[] _parameterRegisters =
        {
            (Register64.RCX, XmmRegister.XMM0),
            (Register64.RDX, XmmRegister.XMM1),
            (Register64.R8, XmmRegister.XMM2),
            (Register64.R9, XmmRegister.XMM3)
        };

        public ExpressionResult Compile(
            FunctionCallExpression expression,
            PreferredRegister target,
            ICompilationContext context
        )
        {
            var parameterStorages = new Storage[Math.Min(expression.Parameters.Length, 4)];
            var parameterSourceTypes = new Type[expression.Parameters.Length];
            Type[] parameterTargetTypes;

            FunctionDeclaration knownDeclaration = null;
            var isIATEntry = false;
            if (expression.Expression is AtomicExpression atomicExpression && atomicExpression.Token.Kind == TokenKind.Identifier)
            {
                knownDeclaration = GetKnownDeclaration(context.Symbols, atomicExpression, ref isIATEntry, out parameterTargetTypes);
                if(knownDeclaration.Parameters.Length != expression.Parameters.Length)
                    throw new BadSignatureException(knownDeclaration);
            }
            else
                parameterTargetTypes = parameterSourceTypes;

            CompileAndStoreParameters(expression, context, parameterSourceTypes, parameterStorages);

            var functionPtr = CompileFunctionPtr(context, expression, target);
            PrepareParameters(context, parameterTargetTypes, parameterSourceTypes, parameterStorages);
            CallPreparedFunction(context, functionPtr, isIATEntry);

            var returnType = knownDeclaration?.ReturnType ?? Constants.LongType; // todo: better alternative if not known
            return new ExpressionResult(returnType, returnType.MakeRegisterWithCorrectSize(Register64.RAX, XmmRegister.XMM0));
        }

        private static FunctionDeclaration GetKnownDeclaration(
            ISymbolResolver scope,
            AtomicExpression atomicExpression,
            ref bool isIATEntry,
            out Type[] parameterTargetTypes
        )
        {
            var identifier = atomicExpression.Token.RawText;
            var knownDeclaration = scope.GetFunctionDeclaration(identifier);
            if (knownDeclaration == null)
            {
                var importFunc = scope.GetFunctionImport(identifier);
                if (importFunc == null)
                    throw new UnknownIdentifierException(identifier);

                knownDeclaration = importFunc.Declaration;
                isIATEntry = true;
            }

            if (knownDeclaration == null)
                throw new UnknownIdentifierException($"Cannot resolve function signature for identifier: \"{identifier}\"");
            parameterTargetTypes = knownDeclaration.Parameters.Select(par => par.ParameterType).ToArray();
            return knownDeclaration;
        }

        private static ExpressionResult CompileFunctionPtr(
            ICompilationContext context,
            FunctionCallExpression expression,
            PreferredRegister target
        )
        {
            var functionPtr = context.CompileExpression(expression.Expression, target);
            if (!functionPtr.ValueType.IsIntegerRegisterType())
                throw new TypeMismatchException(Constants.FunctionPointerType.ToString(), functionPtr.ValueType.ToString());

            if (functionPtr.Kind != ExpressionResult.ResultKind.Offset &&
                (functionPtr.Kind != ExpressionResult.ResultKind.Value ||
                 functionPtr.Value.IsSameRegister(Register64.RCX) ||
                 functionPtr.Value.IsSameRegister(Register64.RDX) ||
                 functionPtr.Value.IsSameRegister(Register64.R8) ||
                 functionPtr.Value.IsSameRegister(Register64.R9)))
            {
                functionPtr.GenerateMoveTo(Register64.RAX, context.Generator, context.Linking);
                functionPtr = new ExpressionResult(functionPtr.ValueType, Register64.RAX);
            }

            return functionPtr;
        }

        private void PrepareParameters(
            ICompilationContext context,
            IReadOnlyList<Type> parameterTargetTypes,
            IReadOnlyList<Type> parameterSourceTypes,
            IReadOnlyList<Storage> parameterStorages
        )
        {
            for (var i = 0; i < Math.Min(_parameterRegisters.Length, parameterStorages.Count); i++)
            {
                var (intRegister, floatRegister) = _parameterRegisters[i];
                var register = parameterTargetTypes[i].MakeRegisterWithCorrectSize(intRegister, floatRegister);
                parameterStorages[i].AsExpressionResult(parameterSourceTypes[i]).GenerateMoveTo(register, parameterTargetTypes[i], context.Generator, context.Linking);
                context.Storage.Release(parameterStorages[i]);
            }

            for (var i = _parameterRegisters.Length; i < parameterStorages.Count; i++)
            {
                var parameterTemp = parameterStorages[i].AsExpressionResult(parameterSourceTypes[i]);
                if (parameterTemp.Kind == ExpressionResult.ResultKind.Value)
                    context.Generator.MovToDereferenced(Register64.RSP, parameterTemp.Value, context.Symbols.GetCalleeParameterOffset(i - 4));
                else
                {
                    parameterTemp.GenerateMoveTo(Register64.R10, context.Generator, context.Linking);
                    context.Generator.MovToDereferenced(Register64.RSP, Register64.R10, context.Symbols.GetCalleeParameterOffset(i - 4));
                }

                context.Storage.Release(parameterStorages[i]);
            }
        }

        private static void CallPreparedFunction(ICompilationContext context, ExpressionResult functionPtr, bool isIATEntry)
        {
            switch (functionPtr.Kind)
            {
                case ExpressionResult.ResultKind.Offset:
                    if (isIATEntry)
                        context.Generator.CallDereferenced4(Constants.DummyOffsetInt);
                    else
                        context.Generator.CallRelative(Constants.DummyOffsetInt);
                    functionPtr.OffsetFixup(context.Linking, context.Generator);
                    break;
                case ExpressionResult.ResultKind.Value:
                    if (isIATEntry)
                        context.Generator.CallDereferenced(functionPtr.Value.AsR64());
                    else
                        context.Generator.Call(functionPtr.Value.AsR64());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void CompileAndStoreParameters(
            FunctionCallExpression expression,
            ICompilationContext context,
            IList<Type> parameterSourceTypes,
            IList<Storage> parameterStorages
        )
        {
            for (var i = 0; i < expression.Parameters.Length; i++) // Compiling parameter expressions
            {
                var parameter = expression.Parameters[i];
                var parameterResult = context.CompileExpression(parameter, context.Storage.MakePreferredRegister());
                parameterSourceTypes[i] = parameterResult.ValueType;

                if (i < 4) // First 4 parameters are passed as RCX, RDX, R8, R9
                {
                    var storage = context.Storage.Allocate(parameterResult.ValueType.IsIntegerRegisterType());
                    storage.Store(parameterResult, context.Generator, context.Linking);
                    parameterStorages[i] = storage;
                }
                else // Remaining parameters are passed on the stack
                {
                    var tempVolatile = parameterResult.GetUnoccupiedVolatile(parameterResult.ValueType);
                    parameterResult.GenerateMoveTo(tempVolatile, context.Generator, context.Linking);
                    var stackReference = new ExpressionResult(
                        parameterResult.ValueType,
                        Register64.RSP,
                        context.Symbols.GetCalleeParameterOffset(i),
                        Segment.SS
                    );
                    stackReference.GenerateAssign(tempVolatile, context.Generator, context.Linking);
                }
            }
        }
    }
}