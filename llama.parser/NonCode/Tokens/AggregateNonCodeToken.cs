namespace Llama.Parser.NonCode.Tokens
{
    using System.Text;
    using Framework;

    internal class AggregateNonCodeToken : INonCode
    {
        private readonly INonCode[] _children;

        public AggregateNonCodeToken(INonCode[] children) => _children = children;

        public void WalkRecursive(ISourceWalker walker)
        {
            foreach (var nonCode in _children)
                walker.Walk(nonCode);
            walker.Walk(this);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var nonCode in _children)
                stringBuilder.Append(nonCode);
            return stringBuilder.ToString();
        }
    }
}