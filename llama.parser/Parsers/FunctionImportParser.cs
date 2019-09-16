namespace Llama.Parser.Parsers
{
    using Lexer;
    using Nodes;

    internal class FunctionImportParser : IParse<FunctionImport>
    {
        public FunctionImport Read(IParseContext context)
        {
            context.ReadOrPanic(TokenKind.Import);
            context.ReadOrPanic(TokenKind.OpenParanthesis);
            var libraryName = context.ReadOrPanic(TokenKind.StringLiteral);
            context.ReadOrPanic(TokenKind.CloseParanthesis);
            var functionDeclaration = context.ReadNode<FunctionDeclaration>();
            return new FunctionImport(libraryName, functionDeclaration);
        }
    }
}