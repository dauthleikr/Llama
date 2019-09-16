namespace Llama.Parser.Nodes
{
    using Lexer;

    internal class FunctionImport
    {
        public Token LibraryName { get; }
        public FunctionDeclaration Declaration { get; }

        public FunctionImport(Token libraryName, FunctionDeclaration declaration)
        {
            LibraryName = libraryName;
            Declaration = declaration;
        }
    }
}