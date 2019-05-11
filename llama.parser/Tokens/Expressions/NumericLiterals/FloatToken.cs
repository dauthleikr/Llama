﻿namespace Llama.Parser.Tokens.Expressions.NumericLiterals
{
    using Language;

    internal class FloatToken : NumericLiteralToken
    {
        private readonly string _token;
        public readonly float Number;

        public FloatToken(string token, float number)
        {
            _token = token;
            Number = number;
        }

        protected override string ToStringInternal() => _token;
        public override BasicType Type => BasicType.Float;
    }
}