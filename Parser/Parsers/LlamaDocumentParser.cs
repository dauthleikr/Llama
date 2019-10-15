namespace Llama.Parser.Parsers
{
    using System.Collections.Generic;
    using Lexer;
    using Nodes;

    internal class LlamaDocumentParser : IParse<LlamaDocument>
    {
        public LlamaDocument Read(IParseContext context)
        {
            var functions = new List<FunctionImplementation>();
            var imports = new List<FunctionImport>();
            while (context.NextCodeToken.Kind != TokenKind.EndOfStream)
                if (context.NextCodeToken.Kind == TokenKind.Import)
                    imports.Add(context.ReadNode<FunctionImport>());
                else
                    functions.Add(context.ReadNode<FunctionImplementation>());
            return new LlamaDocument(functions.ToArray(), imports.ToArray());
        }
    }
}