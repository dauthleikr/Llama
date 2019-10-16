namespace Llama.Linker
{
    internal class IATFixup : IFixupInfo
    {
        public string Library { get; }
        public string Function { get; }
        public long Position { get; set; }

        public IATFixup(long position, string library, string function)
        {
            Library = library;
            Function = function;
            Position = position;
        }
    }
}