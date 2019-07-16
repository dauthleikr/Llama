namespace Llama.Parser.Parsers
{
    using System.Globalization;
    using System.Text;
    using Framework;
    using Tokens.Expressions;
    using Tokens.Expressions.NumericLiterals;

    internal class NumericLiteralParser : AtomicTokenParser<NumericLiteralToken>
    {
        public override bool IsPlausible(ISourcePeeker reader, IParseContext context)
        {
            var peekChar = reader.Peek();
            return char.IsDigit(peekChar) || peekChar == '.' && char.IsDigit(reader.PeekFurther(1));
        }

        protected override ITokenizationResult<NumericLiteralToken> TryReadTokenInternal(ISourceReader reader, IParseContext context) => TryReadNumber(reader);

        private ITokenizationResult<NumericLiteralToken> TryReadNumber(ISourceReader reader)
        {
            var decimapPoint = false;
            var digitAfterDecimalPoint = false;
            var stringBuilder = new StringBuilder();
            var numBuilder = new StringBuilder();

            while (true)
            {
                var peekChar = reader.Peek();
                var isDigit = char.IsDigit(peekChar);
                var isSeperator = peekChar == '_';

                if (peekChar == '.')
                {
                    if (decimapPoint)
                        return Error(reader, "Unexpected second decimal point", 1);
                    decimapPoint = true;
                }
                else if (decimapPoint && isDigit)
                {
                    digitAfterDecimalPoint = true;
                }
                else if (!isDigit && !isSeperator)
                {
                    break;
                }

                reader.Eat();
                stringBuilder.Append(peekChar);
                if (!isSeperator)
                    numBuilder.Append(peekChar);
            }

            if (numBuilder.Length == 0)
                return ErrorExpectedToken(reader);
            if (decimapPoint && !digitAfterDecimalPoint)
                return Error(reader, "Expected digit after decimal point", 1);
            return TryReadSuffixForNumber(reader, stringBuilder, numBuilder.ToString());
        }

        private ITokenizationResult<NumericLiteralToken> TryReadSuffixForNumber(ISourceReader reader, StringBuilder token, string number)
        {
            var suffixChar = reader.ReadChar();
            token.Append(suffixChar);

            switch (char.ToUpper(suffixChar))
            {
                case 'F':
                {
                    if (float.TryParse(number, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var result))
                        return new FloatToken(token.ToString(), result);
                    break;
                }

                case 'D':
                {
                    if (double.TryParse(number, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var result))
                        return new DoubleToken(token.ToString(), result);
                    break;
                }

                case 'U':
                {
                    if (ulong.TryParse(number, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result))
                        return new UI64Token(token.ToString(), result);
                    break;
                }

                default: // no suffix, undo reading and adding the last char
                    reader.Vomit();
                    token.Remove(token.Length - 1, 1);
                    break;
            }

            if (long.TryParse(number, out var defaultResult))
                return new FloatToken(token.ToString(), defaultResult);
            return Error(reader, "", 2);
        }
    }
}