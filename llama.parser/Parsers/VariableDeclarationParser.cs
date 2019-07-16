namespace Llama.Parser.Parsers
{
    using Framework;
    using Tokens;

    internal class VariableDeclarationParser : ParserBase<VariableDeclarationToken>
    {
        public override ITokenizationResult<VariableDeclarationToken> TryReadToken(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser)
        {
            if (context.TryReadToken<IdentifierToken>(out var typeIdentifier) && context.TryReadToken<IdentifierToken>(out var nameIdentifier))
                return new VariableDeclarationToken(typeIdentifier, nameIdentifier);
            return ErrorExpectedToken(reader);
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => context.IsPlausible<IdentifierToken>();
    }
}