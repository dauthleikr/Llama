namespace Llama.Parser.Nodes
{
    public class FunctionImplementation
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