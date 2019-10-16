namespace Llama.Compiler.Cli
{
    using Linker;

    internal class LinkerFactory : IFactory<IAddressFixer>
    {
        public IAddressFixer Create() => new Linker();
    }
}