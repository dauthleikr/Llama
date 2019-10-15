namespace Llama.Parser.Nodes
{
    internal class CodeBlock : IStatement
    {
        public IStatement[] Statements { get; }

        public CodeBlock(params IStatement[] statements) => Statements = statements;
    }
}