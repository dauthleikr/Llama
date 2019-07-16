namespace Llama.Parser.Parsers
{
    using System.Collections.Generic;
    using Abstractions;
    using Entities;
    using Entities.Expressions;
    using Framework;

    public class FunctionCallParser : ParserBase<FunctionCallEntity>
    {
        public override IParseResult<FunctionCallEntity> TryRead(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser)
        {
            if (!context.TryRead<OpenParanthesisEntity>(out var openParanthesis))
                return ErrorExpectedEntity(reader);

            var parameters = new List<IExpressionEntity>();
            var commas = new List<CommaEntity>();
            while (context.TryRead<IExpressionEntity>(out var parameter))
            {
                parameters.Add(parameter);
                if (!context.TryRead<CommaEntity>(out var comma))
                    break;
                commas.Add(comma);
            }

            return context.TryRead<CloseParanthesisEntity>().Match<CloseParanthesisEntity, IParseResult<FunctionCallEntity>>(
                closeParanthesis => new FunctionCallEntity(openParanthesis, parameters, commas, closeParanthesis),
                (err, self) => new ErrorResult<FunctionCallEntity>(err, "Expected ',' or ')'", 1)
            );
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => context.IsPlausible<OpenParanthesisEntity>();
    }
}