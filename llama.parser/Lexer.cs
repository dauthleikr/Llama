namespace Llama.Parser
{
    using System.Collections.Generic;
    using System.Linq;

    public class Lexer
    {
        private readonly ITokenize[] _tokenizes;

        public Lexer(IEnumerable<ITokenize> tokenizers) => _tokenizes = tokenizers.ToArray();

        public Token NextToken(string source, ref int position)
        {
            if (position >= source.Length)
                return new Token(TokenKind.EndOfStream, string.Empty);
            foreach (var tokenizer in _tokenizes)
                if (tokenizer.TryRead(source, ref position, out var token))
                    return token;
            throw new TokenException("No matching token");
        }

        public Token NextNonTriviaToken(string source, ref int position)
        {
            Token token;
            while ((token = NextToken(source, ref position)).IsTrivia) { }

            return token;
        }
    }
}