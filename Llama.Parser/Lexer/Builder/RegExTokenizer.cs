namespace Llama.Parser.Lexer.Builder
{
    using System.Text.RegularExpressions;

    internal class RegExTokenizer : ITokenize
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
}