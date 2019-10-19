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
        public string CurrentFunctionIdentifier { get; }

        private readonly Dictionary<string, FunctionDeclaration> _functionDeclarations;
        private readonly Dictionary<string, FunctionImport> _functionImports;
        private readonly Dictionary<string, int> _localToOffset;
        private LocalScope _scope = new LocalScope();

        private FunctionScope(
            FunctionDeclaration function,
            Dictionary<string, int> localToOffset,
            int calleeParameterSpace,
            IEnumerable<FunctionImport> imports,
            IEnumerable<FunctionDeclaration> declarations
        )
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));
            if (imports == null)
                throw new ArgumentNullException(nameof(imports));
            if (declarations == null)
                throw new ArgumentNullException(nameof(declarations));

            CurrentFunctionIdentifier = function.Identifier.RawText;
            _localToOffset = localToOffset ?? throw new ArgumentNullException(nameof(localToOffset));
            _functionDeclarations = declarations.ToDictionary(item => item.Identifier.RawText, item => item);
            _functionImports = imports.ToDictionary(item => item.Declaration.Identifier.RawText, item => item);
            TotalStackSpace = calleeParameterSpace + (_localToOffset.Any() ? _localToOffset.Values.Max() : 0);
        }

        public int GetCalleeParameterOffset(int index)
        {
            var offset = index * 8;
            if (offset < 0)
            {
                throw new ArgumentException(
                    $"{nameof(FunctionScope)}: {nameof(GetCalleeParameterOffset)}: Cannot get paramter for index {index} (bad index)"
                );
            }

            return offset;
        }

        public int GetLocalOffset(string identifier) => _localToOffset[identifier];

        public Type GetLocalType(string identifier) => _scope.GetLocalType(identifier);

        public ExpressionResult GetLocalReference(string identifier) =>
            new ExpressionResult(GetLocalType(identifier), Register64.RSP, GetLocalOffset(identifier));

        public FunctionDeclaration GetFunctionDeclaration(string identifier) => _functionDeclarations.GetValueOrDefault(identifier);
        public FunctionImport GetFunctionImport(string identifier) => _functionImports.GetValueOrDefault(identifier);

        public bool IsLocalDefined(string identifier) => _scope.IsLocalDefined(identifier);

        public void DefineLocal(string identifier, Type type) => _scope.DefineLocal(identifier, type);

        public void PushScope() => _scope = new LocalScope(_scope);

        public void PopScope() => _scope = _scope.Parent ?? throw new InvalidOperationException();

        public static FunctionScope FromBlock(
            FunctionImplementation function,
            IEnumerable<FunctionImport> imports,
            IEnumerable<FunctionDeclaration> declarations
        )
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));
            if (declarations == null)
                throw new ArgumentNullException(nameof(declarations));

            var localToOffset = new Dictionary<string, int>();

            void DeclareLocal(string name, ref int offset)
            {
                if (localToOffset.ContainsKey(name))
                    throw new Exception($"Cannot redefine {name}");
                localToOffset[name] = offset;
                offset += 8;
            }

            // todo: use (shadow) stack space provided by caller for parameters, for now they are just defined as locals
            var offset = 0;
            foreach (var parameter in function.Declaration.Parameters)
                DeclareLocal(parameter.ParameterIdentifier.RawText, ref offset);

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
                            DeclareLocal(declaration.Identifier.RawText, ref startOffset);
                            break;
                        default:
                            Debug.Fail($"{nameof(GetDeclarationsAndBlocks)} returned neither declaration nor block");
                            break;
                    }
                }
            }

            var maxCalleeParameters = 4; // R9, R8, RDX, RCX home is guaranteed
            foreach (var methodCallExpression in function.Body.GetExpressions().OfType<MethodCallExpression>())
            {
                if (methodCallExpression.Parameters.Length > maxCalleeParameters)
                    maxCalleeParameters = methodCallExpression.Parameters.Length;
            }

            var calleeParameterSpace = maxCalleeParameters * 8;
            SetLocalOffsets(calleeParameterSpace, function.Body);

            var scope = new FunctionScope(function.Declaration, localToOffset, calleeParameterSpace, imports, declarations);
            foreach (var parameter in function.Declaration.Parameters)
                scope.DefineLocal(parameter.ParameterIdentifier.RawText, parameter.ParameterType);
            return scope;
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
                                @for.Variable
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