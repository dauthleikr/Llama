namespace Llama.Parser.Parsers
{
    using Abstractions;
    using Entities;
    using Framework;

    internal class VariableDeclarationParser : ParserBase<VariableDeclarationEntity>
    {
        public override IParseResult<VariableDeclarationEntity> TryRead(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser)
        {
            if (context.TryRead<IdentifierEntity>(out var typeIdentifier) && context.TryRead<IdentifierEntity>(out var nameIdentifier))
                return new VariableDeclarationEntity(typeIdentifier, nameIdentifier);
            return ErrorExpectedToken(reader);
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => context.IsPlausible<IdentifierEntity>();
    }
}