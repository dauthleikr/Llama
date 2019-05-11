using System;
using System.Collections.Generic;
using System.Text;

namespace llama.syntax.Tokens
{
    public class ExpressionFunctionCallToken : ExpressionToken
    {
        public readonly ExpressionIdentifierToken FunctionIdentifier;
        public readonly OpenParanthesisToken ParameterOpenParanthesis;
        public readonly IReadOnlyList<ExpressionToken> Parameters;
        public readonly IReadOnlyList<CommaToken> ParameterCommas;
        public readonly CloseParanthesisToken ParameterCloseParanthesis;

        public ExpressionFunctionCallToken(ExpressionIdentifierToken functionIdentifier, OpenParanthesisToken parameterOpenParanthesis, IReadOnlyList<ExpressionToken> parameters, IReadOnlyList<CommaToken> parameterCommas, CloseParanthesisToken parameterCloseParanthesis)
        {
            FunctionIdentifier = functionIdentifier;
            ParameterOpenParanthesis = parameterOpenParanthesis;
            Parameters = parameters;
            ParameterCloseParanthesis = parameterCloseParanthesis;
            ParameterCommas = parameterCommas;

            if (Parameters.Count != 0 && Parameters.Count - 1 != ParameterCommas.Count)
                throw new ArgumentException("Parameter - Comma mismatch");
        }

        public override void WriteSourceRecursive(ITextOutput textOutput, bool codeOnly = true)
        {
            FunctionIdentifier.WriteSourceRecursive(textOutput, codeOnly);
            ParameterOpenParanthesis.WriteSourceRecursive(textOutput, codeOnly);
            if (Parameters.Count > 0)
                Parameters[0].WriteSourceRecursive(textOutput, codeOnly);
            for (var i = 0; i < ParameterCommas.Count; i++)
            {
                ParameterCommas[i].WriteSourceRecursive(textOutput, codeOnly);
                Parameters[i + 1].WriteSourceRecursive(textOutput, codeOnly);
            }
            ParameterCloseParanthesis.WriteSourceRecursive(textOutput, codeOnly);
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            ParameterOpenParanthesis.WalkRecursive(walker, codeChildrenOnly);
            if (Parameters.Count > 0)
                Parameters[0].WalkRecursive(walker, codeChildrenOnly);
            for (var i = 0; i < ParameterCommas.Count; i++)
            {
                ParameterCommas[i].WalkRecursive(walker, codeChildrenOnly);
                Parameters[i + 1].WalkRecursive(walker, codeChildrenOnly);
            }
            ParameterCloseParanthesis.WalkRecursive(walker, codeChildrenOnly);
            FunctionIdentifier.WalkRecursive(walker, codeChildrenOnly);
            base.WalkRecursive(walker, codeChildrenOnly);
        }

        public static bool TryParse(ISourceReader reader, out ExpressionFunctionCallToken result)
        {
            if (ExpressionIdentifierToken.TryParse(reader, out var functionIdentifier))
                return TryParseAfterIdentifier(reader, functionIdentifier, out result);

            result = null;
            return false;
        }

        public static bool TryParseAfterIdentifier(ISourceReader reader, ExpressionIdentifierToken previous, out ExpressionFunctionCallToken result)
        {
            var start = reader.Position;
            if (!OpenParanthesisToken.TryParse(reader, out var openParanthesisToken))
            {
                result = null;
                return false;
            }

            var parameters = new List<ExpressionToken>();
            var parameterCommas = new List<CommaToken>();
            while (!reader.IsAtEnd)
            {
                if (!ExpressionToken.TryParse(reader, out var parameterExpressionToken))
                    throw new TokenizerException(reader.Position, $"Bad function argument expression");
                parameters.Add(parameterExpressionToken);
                if (!CommaToken.MayStartWith(reader.Peek()))
                {
                    if (CloseParanthesisToken.MayStartWith(reader.Peek()))
                        break;
                    throw new TokenizerException(reader.Position, $"Expected end of function call or next parameter");
                }
                if (!CommaToken.TryParse(reader, out var commaToken))
                    throw new TokenizerException(reader.Position, "Expected comma (parameter seperator)");
                parameterCommas.Add(commaToken);
            }

            if (!CloseParanthesisToken.TryParse(reader, out var closeParanthesisToken))
            {
                reader.Position = start;
                result = null;
                return false;
            }

            result = new ExpressionFunctionCallToken(previous, openParanthesisToken, parameters, parameterCommas, closeParanthesisToken);
            return true;
        }

        public static bool MayStartWith(char character) => ExpressionIdentifierToken.MayStartWith(character);
        public static bool MayStartWithAfterIdentifier(char character) => OpenParanthesisToken.MayStartWith(character);
    }
}
