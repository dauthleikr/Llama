namespace Llama.Parser.Abstractions
{
    internal interface ITokenize<T> where T: IToken
    {
        bool TryRead<T>()
    }
}