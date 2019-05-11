namespace Llama.Parser.Parsers
{
    using System.Collections.Generic;
    using Framework;
    using Tokens;
    using Tokens.Expressions;

    internal class FunctionCallParser : ParserBase<FunctionCallToken>
    {
        public override ITokenizationResult<FunctionCallToken> TryReadToken(ISourceReader reader, IParseContext context, INonCodeParser nonCodeParser)
        {
            if (!context.TryReadToken<OpenParanthesisToken>(out var openParanthesis))
                return ErrorExpectedToken(reader);

            var parameters = new List<IExpressionToken>();
            var commas = new List<CommaToken>();
            while (context.TryReadToken<IExpressionToken>(out var parameter))
            {
                parameters.Add(parameter);
                if (!context.TryReadToken<CommaToken>(out var comma))
                    break;
                commas.Add(comma);
            }

            return context.TryReadToken<CloseParanthesisToken>().Match<CloseParanthesisToken, ITokenizationResult<FunctionCallToken>>(
                closeParanthesis => new FunctionCallToken(openParanthesis, parameters, commas, closeParanthesis),
                (err, self) => new ErrorResult<FunctionCallToken>(err, "Expected ',' or ')'", 1)
            );
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => context.IsPlausible<OpenParanthesisToken>();
    }
}