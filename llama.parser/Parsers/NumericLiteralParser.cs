namespace Llama.Parser.Parsers
{
    using System.Globalization;
    using System.Text;
    using Abstractions;
    using Entities.Expressions;
    using Entities.Expressions.NumericLiterals;
    using Framework;

    internal class NumericLiteralParser : AtomicEntityParser<NumericLiteralEntity>
    {
        public override bool IsPlausible(ISourcePeeker reader, IParseContext context)
        {
            var peekChar = reader.Peek();
            return char.IsDigit(peekChar) || peekChar == '.' && char.IsDigit(reader.PeekFurther(1));
        }

        protected override IParseResult<NumericLiteralEntity> TryReadEntityInternal(ISourceReader reader, IParseContext context) => TryReadNumber(reader);

        private IParseResult<NumericLiteralEntity> TryReadNumber(ISourceReader reader)
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
                return ErrorExpectedEntity(reader);
            if (decimapPoint && !digitAfterDecimalPoint)
                return Error(reader, "Expected digit after decimal point", 1);
            return TryReadSuffixForNumber(reader, stringBuilder, numBuilder.ToString());
        }

        private IParseResult<NumericLiteralEntity> TryReadSuffixForNumber(ISourceReader reader, StringBuilder token, string number)
        {
            var suffixChar = reader.ReadChar();
            token.Append(suffixChar);

            switch (char.ToUpper(suffixChar))
            {
                case 'F':
                {
                    if (float.TryParse(number, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var result))
                        return new FloatEntity(token.ToString(), result);
                    break;
                }

                case 'D':
                {
                    if (double.TryParse(number, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var result))
                        return new DoubleEntity(token.ToString(), result);
                    break;
                }

                case 'U':
                {
                    if (ulong.TryParse(number, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var result))
                        return new UI64Entity(token.ToString(), result);
                    break;
                }

                default: // no suffix, undo reading and adding the last char
                    reader.Vomit();
                    token.Remove(token.Length - 1, 1);
                    break;
            }

            if (long.TryParse(number, out var defaultResult))
                return new FloatEntity(token.ToString(), defaultResult);
            return Error(reader, "", 2);
        }
    }
}