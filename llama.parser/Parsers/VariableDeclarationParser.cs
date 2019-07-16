namespace Llama.Parser.Parsers
{
    using Abstractions;
    using Entities;
    using Framework;

    public class VariableDeclarationParser : ParserBase<VariableDeclarationEntity>
    {
        public override IParseResult<VariableDeclarationEntity> TryRead(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser)
        {
            if (!context.TryRead<IdentifierEntity>(out var typeIdentifier) || !context.TryRead<IdentifierEntity>(out var nameIdentifier))
                return ErrorExpectedEntity(reader);

            if (context.TryRead<AssignmentEntity>(out var equals) && context.TryRead<IExpressionEntity>(out var expression))
                return new VariableInitializationEntity(typeIdentifier, nameIdentifier, equals, expression);
            return new VariableDeclarationEntity(typeIdentifier, nameIdentifier);
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => context.IsPlausible<IdentifierEntity>();
    }
}