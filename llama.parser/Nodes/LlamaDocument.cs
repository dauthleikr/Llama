namespace Llama.Parser.Nodes
{
    public class LlamaDocument
    {
        public FunctionImplementation[] Functions { get; }
        public FunctionImport[] Imports { get; }

        public LlamaDocument(FunctionImplementation[] functions, FunctionImport[] imports)
        {
            Functions = functions;
            Imports = imports;
        }
    }
}