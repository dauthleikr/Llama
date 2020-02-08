namespace Llama.Compiler.Cli
{
    using Linker;

    internal class LinkerFactory : IFactory<ILinkingInfo>
    {
        public ILinkingInfo Create() => new Linker();
    }
}