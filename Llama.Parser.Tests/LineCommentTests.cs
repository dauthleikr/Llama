namespace Llama.Parser.Tests
{
    using DebugImplementations;
    using NonCode.Parsers;
    using NonCode.Tokens;
    using NUnit.Framework;

    [TestFixture]
    internal class LineCommentTests
    {
        [Test]
        public void ParsesLineComment()
        {
            const string src = "//wow";
            var reader = new StringSourceReader(src);
            var success = new LineCommentParser().TryParse(reader, out LineCommentToken result);

            Assert.True(success);
            Assert.NotNull(result);
            Assert.AreEqual(src, result.ToString());
        }

        [Test]
        public void StopsAtLineEnding()
        {
            const string src = "//wow\nbad";
            var reader = new StringSourceReader(src);
            var success = new LineCommentParser().TryParse(reader, out LineCommentToken result);

            Assert.True(success);
            Assert.NotNull(result);
            Assert.AreNotEqual(src, result.ToString());
            Assert.AreEqual(src.Split('\n')[0] + "\n", result.ToString());
        }
    }
}