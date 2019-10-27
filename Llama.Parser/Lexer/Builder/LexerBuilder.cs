namespace Llama.Parser.Lexer
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Builder;

    public class LexerBuilder
    {
        private readonly List<ITokenize> _greedyTokenizers = new List<ITokenize>();
        private readonly List<ITokenize> _staticTokenizers = new List<ITokenize>();
        private readonly List<ISkipTrivia> _triviaSkippers = new List<ISkipTrivia>();

        public void AddStaticToken(TokenKind kind, string staticText) => _staticTokenizers.Add(new StaticTextTokenizer(staticText, kind));
        public void AddRegexToken(TokenKind kind, Regex regex) => _greedyTokenizers.Add(new RegExTokenizer(regex, kind));
        public void AddTrivia(TokenKind kind, Regex regex) => _triviaSkippers.Add(new RegExTriviaSkipper(regex));
        public Lexer Build() => new Lexer(_triviaSkippers, _staticTokenizers, _greedyTokenizers);

        public static Lexer BuildLlamaLexer()
        {
            var builder = new LexerBuilder();

            builder.AddTrivia(TokenKind.LineComment, new Regex(@"//[^\n\r]*", RegexOptions.Compiled));
            builder.AddTrivia(TokenKind.BlockComment, new Regex(@"\/\*(\*(?!\/)|[^*])*\*\/", RegexOptions.Compiled));
            builder.AddTrivia(TokenKind.WhitespaceOrControl, new Regex(@"[\s\t\n\r]+", RegexOptions.Compiled));

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
            builder.AddStaticToken(TokenKind.PrimitiveType, "void");
            builder.AddStaticToken(TokenKind.PrimitiveType, "int");
            builder.AddStaticToken(TokenKind.PrimitiveType, "long");
            builder.AddStaticToken(TokenKind.PrimitiveType, "short");
            builder.AddStaticToken(TokenKind.PrimitiveType, "byte");
            builder.AddStaticToken(TokenKind.PrimitiveType, "sbyte");
            builder.AddStaticToken(TokenKind.PrimitiveType, "cstr");
            builder.AddStaticToken(TokenKind.PrimitiveType, "float");
            builder.AddStaticToken(TokenKind.PrimitiveType, "double");
            builder.AddStaticToken(TokenKind.PrimitiveType, "bool");
            builder.AddRegexToken(TokenKind.StringLiteral, new Regex("\\G\"[^\"]*\"", RegexOptions.Compiled));
            builder.AddRegexToken(TokenKind.FloatLiteral, new Regex(@"\G([0-9]+[0-9_]*)?\.[0-9]+[0-9_]*", RegexOptions.Compiled));
            builder.AddRegexToken(TokenKind.IntegerLiteral, new Regex("\\G[0-9]+[0-9_]*", RegexOptions.Compiled));
            builder.AddRegexToken(TokenKind.Identifier, new Regex("\\G[_A-Za-z][_A-Za-z0-9]*", RegexOptions.Compiled));

            return builder.Build();
        }
    }
}