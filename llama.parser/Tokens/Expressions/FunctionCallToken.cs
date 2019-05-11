using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Tokens.Expressions
{
    using Framework;

    class FunctionCallToken : TokenBase<FunctionCallToken>
    {
        public readonly OpenParanthesisToken OpenParanthesisToken;
        public readonly IReadOnlyList<IExpressionToken> Parameters;
        public readonly IReadOnlyList<CommaToken> Commas;
        public readonly CloseParanthesisToken CloseParanthesisToken;

        public FunctionCallToken(OpenParanthesisToken open, IReadOnlyList<IExpressionToken> parameters, IReadOnlyList<CommaToken> commaTokens, CloseParanthesisToken close)
        {
            OpenParanthesisToken = open;
            Parameters = parameters;
            Commas = commaTokens;
            CloseParanthesisToken = close;

            if (parameters.Count - 1 != commaTokens.Count && !(parameters.Count == 0 && commaTokens.Count == 0))
                throw new Exception($"{parameters.Count} parameters and {commaTokens.Count} commas in {nameof(FunctionCallToken)}");
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            walker.Walk(this);
            OpenParanthesisToken.WalkRecursive(walker, codeChildrenOnly);
            for (var index = 0; index < Parameters.Count; index++)
            {
                Parameters[index].WalkRecursive(walker, codeChildrenOnly);
                if (index < Commas.Count)
                    Commas[index].WalkRecursive(walker, codeChildrenOnly);
            }
            CloseParanthesisToken.WalkRecursive(walker, codeChildrenOnly);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(OpenParanthesisToken);
            for (var i = 0; i < Parameters.Count; i++)
            {
                builder.Append(Parameters[i]);
                if (Commas.Count > i)
                    builder.Append(Commas[i]);
            }
            builder.Append(CloseParanthesisToken);
            return builder.ToString();
        }
    }
}
