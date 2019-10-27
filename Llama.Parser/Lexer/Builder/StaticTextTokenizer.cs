namespace Llama.Parser.Lexer.Builder
{
    internal class StaticTextTokenizer : ITokenize
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

        public bool TryRead(string src, int pos, out Token result)
        {
            for (var i = 0; i < _text.Length; i++)
            {
                if (_text[i] != src[pos + i])
                {
                    result = default;
                    return false;
                }
            }

            result = new Token(_kind, _textString);
            return true;
        }
    }
}