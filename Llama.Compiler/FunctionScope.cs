namespace Llama.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Parser.Nodes;
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
            var neededSpace = calleeParameterSpace + localToOffset.Values.Max() + 8;
            if (neededSpace % 16 != 8) // make aligned by 8 but not by 16 (call will align)
                neededSpace += 16 - neededSpace % 16;
            TotalStackSpace = neededSpace;
        }

        public int GetLocalOffset(string identifier) => _localToOffset[identifier];

        public Type GetLocalType(string identifier) => _scope.GetLocalType(identifier);

        public bool IsLocalDefined(string identifier) => _scope.IsLocalDefined(identifier);

        public void DefineLocal(string identifier, Type type) => _scope.DefineLocal(identifier, type);

        public void PushScope() => _scope = new LocalScope(_scope);

        public void PopScope() => _scope = _scope.Parent;

        public static FunctionScope FromBlock(CodeBlock block)
        {
            var localToOffset = new Dictionary<string, int>();

            void SetLocalOffsets(int startOffset, IStatement code)
            {
                var offset = startOffset;
                foreach (var statement in GetDeclarationsAndBlocks(code))
                    switch (statement)
                    {
                        case CodeBlock childBlock:
                            SetLocalOffsets(offset, childBlock);
                            break;
                        case Declaration declaration:
                        {
                            var declarationName = declaration.Identifier.RawText;
                            if (localToOffset.ContainsKey(declarationName))
                                throw new Exception($"Cannot redefine {declarationName}");
                            localToOffset[declarationName] = offset;
                            offset += declaration.Type.SizeOf();
                        }
                            break;
                    }
            }

            SetLocalOffsets(0, block);
            var maxCalleeParameters = 4; // R9, R8, RDX, RCX home is guaranteed
            foreach (var methodCallExpression in block.GetExpressions().OfType<MethodCallExpression>())
                if (methodCallExpression.Parameters.Length > maxCalleeParameters)
                    maxCalleeParameters = methodCallExpression.Parameters.Length;

            return new FunctionScope(localToOffset, maxCalleeParameters);
        }

        private static IEnumerable<IStatement> GetDeclarationsAndBlocks(IStatement block)
        {
            foreach (var statement in block.StatementAsBlock().Statements)
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