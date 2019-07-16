namespace Llama.Parser.Abstractions
{
    public interface IAtomicEntity : IEntity
    {
        INonCode PreNonCode { get; set; }
        INonCode PostNonCode { get; set; }
    }
}