namespace Llama.Parser.Parsers
{
    using Nodes;

    internal class FunctionImplementationParser : IParse<FunctionImplementation>
    {
        public FunctionImplementation Read(IParseContext context) =>
            new FunctionImplementation(context.ReadNode<FunctionDeclaration>(), context.ReadNode<IStatement>());
    }
}