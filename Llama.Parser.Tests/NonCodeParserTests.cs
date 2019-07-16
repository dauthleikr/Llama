namespace Llama.Parser.Tests
{
    using DebugImplementations;
    using NonCode;
    using NonCode.Entities;
    using NonCode.Parsers;
    using NUnit.Framework;

    [TestFixture]
    internal class NonCodeParserTests
    {
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

        private NonCodeParser _parser;


        [Test]
        public void ParsesBlockComments()
        {
            const string src = "/*wow*/";
            var reader = new StringSourceReader(src);
            var nonCode = _parser.ReadOrNull(reader);

            Assert.IsAssignableFrom<BlockCommentEntity>(nonCode);
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