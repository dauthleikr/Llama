namespace Llama.Parser.NonCode
{
    using Framework;

    public abstract class NonCodeTokenParserBase<T> : IParseNonCode<T>, IParseNonCode where T : class, INonCode
    {
        public bool TryParse(ISourceReader reader, out INonCode nonCode)
        {
            var success = TryParse(reader, out T nonCodeGeneric);
            nonCode = nonCodeGeneric;
            return success;
        }

        public abstract bool TryParse(ISourceReader reader, out T nonCode);
    }
}