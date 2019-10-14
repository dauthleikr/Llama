namespace Llama.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Extensions;
    using Parser.Nodes;
    using spit;
    using Type = Parser.Nodes.Type;

    public class FunctionScope : IScopeContext
    {
        private class LocalScope
        {
            private readonly Dictionary<string, Type> _definedLocalTypes = new Dictionary<string, Type>();
            public readonly LocalScope Parent;

            public LocalScope(LocalScope parent = null) => Parent = parent;

            public Type GetLocalType(string identifier) =>
                _definedLocalTypes.TryGetValue(identifier, out var type) ? type : Parent?.GetLocalType(identifier);

            public bool IsLocalDefined(string identifier)
            {
                if (_definedLocalTypes.ContainsKey(identifier))
                    return true;

                return Parent?.IsLocalDefined(identifier) ?? false;
            }

            public void DefineLocal(string identifier, Type type) => _definedLocalTypes[identifier] = type;
        }

        public int TotalStackSpace { get; }
        private readonly Dictionary<string, int> _localToOffset;
        private LocalScope _scope = new LocalScope();

        public FunctionScope(Dictionary<string, int> localToOffset, int calleeParameterSpace)
        {
            _localToOffset = localToOffset ?? throw new ArgumentNullException(nameof(localToOffset));
            TotalStackSpace = _localToOffset.Values.Max() + 8 + calleeParameterSpace;
        }

        public int GetLocalOffset(string identifier) => _localToOffset[identifier];

        public Type GetLocalType(string identifier) => _scope.GetLocalType(identifier);

        public ExpressionResult GetLocalReference(string identifier) =>
            new ExpressionResult(GetLocalType(identifier), Register64.RSP, GetLocalOffset(identifier));

        public bool IsLocalDefined(string identifier) => _scope.IsLocalDefined(identifier);

        public void DefineLocal(string identifier, Type type) => _scope.DefineLocal(identifier, type);

        public void PushScope() => _scope = new LocalScope(_scope);

        public void PopScope() => _scope = _scope.Parent ?? throw new InvalidOperationException();

        public static FunctionScope FromBlock(FunctionImplementation function)
        {
            var localToOffset = new Dictionary<string, int>();

            void DeclareLocal(Type type, string name, ref int offset)
            {
                if (localToOffset.ContainsKey(name))
                    throw new Exception($"Cannot redefine {name}");
                localToOffset[name] = offset;
                offset += type.SizeOf();
            }

            var offset = 0;
            foreach (var parameter in function.Declaration.Parameters)
                DeclareLocal(parameter.ParameterType, parameter.ParameterIdentifier.RawText, ref offset);

            void SetLocalOffsets(int startOffset, IStatement code)
            {
                foreach (var statement in GetDeclarationsAndBlocks(code))
                {
                    switch (statement)
                    {
                        case CodeBlock childBlock:
                            SetLocalOffsets(startOffset, childBlock);
                            break;
                        case Declaration declaration:
                        {
                            var declarationName = declaration.Identifier.RawText;
                            if (localToOffset.ContainsKey(declarationName))
                                throw new Exception($"Cannot redefine {declarationName}");
                            localToOffset[declarationName] = startOffset;
                            startOffset += declaration.Type.SizeOf();
                        }
                            break;
                        default:
                            Debug.Fail($"{nameof(GetDeclarationsAndBlocks)} returned neither declaration nor block");
                            break;
                    }
                }
            }

            SetLocalOffsets(offset, function.Body);
            var maxCalleeParameters = 4; // R9, R8, RDX, RCX home is guaranteed
            foreach (var methodCallExpression in function.Body.GetExpressions().OfType<MethodCallExpression>())
            {
                if (methodCallExpression.Parameters.Length > maxCalleeParameters)
                    maxCalleeParameters = methodCallExpression.Parameters.Length;
            }

            return new FunctionScope(localToOffset, maxCalleeParameters);
        }

        private static IEnumerable<IStatement> GetDeclarationsAndBlocks(IStatement block)
        {
            foreach (var statement in block.StatementAsBlock().Statements)
            {
                switch (statement)
                {
                    case For @for:
                    {
                        var forScopeStatements = new[]
                            {
                                @for.Instruction
                            }.Concat(@for.Instruction.StatementAsBlock().Statements)
                            .ToArray();
                        yield return new CodeBlock(forScopeStatements);
                    }
                        break;
                    case If @if:
                        yield return @if.Instruction.StatementAsBlock();
                        if (@if.ElseInstruction != null)
                            yield return @if.ElseInstruction.StatementAsBlock();
                        break;
                    case While @while:
                        yield return @while.Instruction.StatementAsBlock();
                        break;
                    case Declaration declaration:
                        yield return declaration;
                        break;
                    case CodeBlock codeBlock:
                        yield return codeBlock;
                        break;
                }
            }
        }
    }
}