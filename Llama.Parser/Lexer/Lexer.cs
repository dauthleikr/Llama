namespace Llama.Parser.Lexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Lexer
    {
        private readonly ITokenize[] _greedyTokenizers;
        private readonly ITokenize[] _staticTokenizers;
        private readonly ISkipTrivia[] _triviaSkippers;

        public Lexer(IEnumerable<ISkipTrivia> triviaSkippers, IEnumerable<ITokenize> staticTokenizers, IEnumerable<ITokenize> greedyTokenizers)
        {
            _triviaSkippers = triviaSkippers.ToArray();
            _greedyTokenizers = greedyTokenizers.ToArray();
            _staticTokenizers = staticTokenizers.ToArray();
        }

        public Token NextToken(string source, ref int position)
        {
            if (position >= source.Length)
                return new Token(TokenKind.EndOfStream, string.Empty);

            // Skip any trivia
            var didSkipTrivia = true;
            while (didSkipTrivia)
            {
                didSkipTrivia = false;
                foreach (var triviaSkipper in _triviaSkippers)
                {
                    var positionAfterTrivia = triviaSkipper.GetPositionAfterTrivia(source, position);
                    if (positionAfterTrivia > position)
                    {
                        didSkipTrivia = true;
                        position = positionAfterTrivia;
                        break;
                    }
                }
            }

            if (position >= source.Length)
                return new Token(TokenKind.EndOfStream, string.Empty);

            // First, try to find a perfectly matching token
            var internalPositionStatic = position;
            Token staticResult = default;
            foreach (var tokenizer in _staticTokenizers)
            {
                if (!tokenizer.TryRead(source, ref internalPositionStatic, out var token))
                    continue;

                staticResult = token;
                break;
            }

            /*
              Then, try to find a better one with a non-static implementation (e.g. RegEx)
              This is done because an identifier called 'intToString' could be read as type 'int' 
                and identifier 'ToString' because there are no rules that tokens need to be seperated somehow.
              With this implementation, a greedy identifier-tokenizers would read 'intToString' as one, 
                and it would be deemed better than 'int' based on token length
             */
            foreach (var tokenizer in _greedyTokenizers)
            {
                var internalPositionGreedy = position;
                if (!tokenizer.TryRead(source, ref internalPositionGreedy, out var token))
                    continue;

                if (token.RawText.Length > (staticResult.RawText?.Length ?? 0))
                {
                    position = internalPositionGreedy;
                    return token;
                }
            }

            if (staticResult != default)
            {
                position = internalPositionStatic;
                return staticResult;
            }

            static string TakeSome(string str, int maxChars) => str.Substring(0, Math.Min(str.Length, maxChars));
            throw new LexerException($"No matching token: {TakeSome(source.Substring(position), 10)} ...");
        }
    }
}