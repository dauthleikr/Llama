namespace Llama.Parser.Framework
{
    public interface IAtomicToken : IToken
    {
        INonCode PreNonCode { get; set; }
        INonCode PostNonCode { get; set; }
    }
}