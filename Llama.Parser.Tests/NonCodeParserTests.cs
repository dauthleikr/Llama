using System;

namespace Llama.Parser.Tests
{
    using DebugImplementations;
    using NonCode;
    using NonCode.Parsers;
    using NonCode.Tokens;
    using NUnit.Framework;

    [TestFixture]
    internal class NonCodeParserTests
    {
        private NonCodeParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new NonCodeParser(new IParseNonCode[]
            {
                new BlockCommentParser(),
                new LineCommentParser(),
                new WhitespaceParser()
            });
        }


        [Test]
        public void ParsesBlockComments()
        {
            const string src = "/*wow*/";
            var reader = new StringSourceReader(src);
            var nonCode = _parser.ReadOrNull(reader);

            Assert.IsAssignableFrom<BlockCommentToken>(nonCode);
            Assert.AreEqual(nonCode.ToString(), src);
        }

        [Test]
        public void ReturnsNullOnCode()
        {
            const string src = "abc()";
            var reader = new StringSourceReader(src);
            var nonCode = _parser.ReadOrNull(reader);

            Assert.Null(nonCode);
        }
    }
}
