namespace Llama.Parser.Nodes
{
    internal class FunctionImplementation
    {
        public FunctionDeclaration Declaration { get; }
        public IStatement Body { get; }

        public FunctionImplementation(FunctionDeclaration declaration, IStatement body)
        {
            Declaration = declaration;
            Body = body;
        }
    }
}