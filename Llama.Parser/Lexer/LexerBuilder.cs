namespace Llama.Parser.Lexer
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class LexerBuilder
    {
        private class StaticTextTokenizer : ITokenize
        {
            private readonly TokenKind _kind;
            private readonly char[] _text;
            private readonly string _textString;

            public StaticTextTokenizer(string text, TokenKind kind)
            {
                _text = text.ToCharArray();
                _textString = text;
                _kind = kind;
            }

            public bool TryRead(string src, ref int pos, out Token result)
            {
                for (var i = 0; i < _text.Length; i++)
                {
                    if (_text[i] != src[pos + i])
                    {
                        result = default;
                        return false;
                    }
                }

                pos += _text.Length;
                result = new Token(_kind, _textString);
                return true;
            }
        }

        private class RegExTokenizer : ITokenize
        {
            private readonly TokenKind _kind;
            private readonly Regex _regex;

            public RegExTokenizer(Regex regex, TokenKind kind)
            {
                _regex = regex;
                _kind = kind;
            }

            public bool TryRead(string src, ref int pos, out Token result)
            {
                var match = _regex.Match(src, pos);
                if (!match.Success || match.Index > pos)
                {
                    result = default;
                    return false;
                }

                pos += match.Length;
                result = new Token(_kind, match.Value);
                return true;
            }
        }

        private readonly List<ITokenize> _tokenizers = new List<ITokenize>();

        public void AddStaticToken(TokenKind kind, string staticText) => _tokenizers.Add(new StaticTextTokenizer(staticText, kind));
        public void AddRegexToken(TokenKind kind, Regex regex) => _tokenizers.Add(new RegExTokenizer(regex, kind));
        public Lexer Build() => new Lexer(_tokenizers);

        public static Lexer BuildLlamaLexer()
        {
            var builder = new LexerBuilder();

            /*
             * todo: bug: because the builder will always try to match in the given order, longer tokens
             * that contain shorter tokens need to come first, or else it will always match the short token
             * rework that logic sometimes.
             */

            builder.AddRegexToken(TokenKind.LineComment, new Regex(@"//[^\n\r]*", RegexOptions.Compiled));
            builder.AddRegexToken(TokenKind.BlockComment, new Regex(@"\/\*(\*(?!\/)|[^*])*\*\/", RegexOptions.Compiled));
            builder.AddRegexToken(TokenKind.WhitespaceOrControl, new Regex(@"[\s\t\n\r]+", RegexOptions.Compiled));
            builder.AddStaticToken(TokenKind.Equals, "==");
            builder.AddStaticToken(TokenKind.NotEquals, "!=");
            builder.AddStaticToken(TokenKind.GreaterEquals, ">=");
            builder.AddStaticToken(TokenKind.SmallerEquals, "<=");
            builder.AddStaticToken(TokenKind.Assignment, "=");
            builder.AddStaticToken(TokenKind.OpenAngularBracket, "<");
            builder.AddStaticToken(TokenKind.CloseAngularBracket, ">");
            builder.AddStaticToken(TokenKind.Not, "!");
            builder.AddStaticToken(TokenKind.AddressOf, "&");
            builder.AddStaticToken(TokenKind.OpenParanthesis, "(");
            builder.AddStaticToken(TokenKind.CloseParanthesis, ")");
            builder.AddStaticToken(TokenKind.OpenBraces, "{");
            builder.AddStaticToken(TokenKind.CloseBraces, "}");
            builder.AddStaticToken(TokenKind.OpenSquareBracket, "[");
            builder.AddStaticToken(TokenKind.CloseSquareBracket, "]");
            builder.AddStaticToken(TokenKind.Comma, ",");
            builder.AddStaticToken(TokenKind.SemiColon, ";");
            builder.AddStaticToken(TokenKind.Pointer, "*");
            builder.AddStaticToken(TokenKind.Plus, "+");
            builder.AddStaticToken(TokenKind.Minus, "-");
            builder.AddStaticToken(TokenKind.Divide, "/");
            builder.AddStaticToken(TokenKind.Modolu, "%");
            builder.AddStaticToken(TokenKind.True, "true");
            builder.AddStaticToken(TokenKind.False, "false");
            builder.AddStaticToken(TokenKind.New, "new");
            builder.AddStaticToken(TokenKind.Delete, "delete");
            builder.AddStaticToken(TokenKind.Import, "import");
            builder.AddStaticToken(TokenKind.If, "if");
            builder.AddStaticToken(TokenKind.Else, "else");
            builder.AddStaticToken(TokenKind.While, "while");
            builder.AddStaticToken(TokenKind.For, "for");
            builder.AddStaticToken(TokenKind.Return, "return");
            builder.AddRegexToken(TokenKind.PrimitiveType, new Regex("(void|int|long|short|byte|sbyte|cstr|float|double|bool)", RegexOptions.Compiled));
            builder.AddRegexToken(TokenKind.StringLiteral, new Regex("\"[^\"]*\"", RegexOptions.Compiled));
            builder.AddRegexToken(TokenKind.IntegerLiteral, new Regex("[0-9]+[0-9_]*", RegexOptions.Compiled));
            builder.AddRegexToken(TokenKind.FloatLiteral, new Regex(@"([0-9]+[0-9_]*)?\.[0-9]+[0-9_]*", RegexOptions.Compiled));
            builder.AddRegexToken(TokenKind.Identifier, new Regex("[_A-Za-z][_A-Za-z0-9]*", RegexOptions.Compiled));
            
            return builder.Build();
        }
    }
}