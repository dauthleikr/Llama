namespace Llama.Parser.Lexer.Builder
{
    using System.Text.RegularExpressions;

    internal class RegExTriviaSkipper : ISkipTrivia
    {
        private readonly Regex _regex;

        public RegExTriviaSkipper(Regex regex)
        {
            _regex = regex;
        }

        public int GetPositionAfterTrivia(string src, int pos)
        {
            var match = _regex.Match(src, pos);
            if (!match.Success || match.Index > pos)
                return pos;

            return pos + match.Length;
        }
    }
}