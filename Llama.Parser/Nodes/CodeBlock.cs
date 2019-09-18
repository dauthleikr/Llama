namespace Llama.Parser.Nodes
{
    public class CodeBlock : IStatement
    {
        public IStatement[] Statements { get; }

        public CodeBlock(params IStatement[] statements) => Statements = statements;
    }
}