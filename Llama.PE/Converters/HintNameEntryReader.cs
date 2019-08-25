namespace Llama.PE.Converters
{
    using System.Text;
    using BinaryUtils;
    using Structures.Sections.Idata;

    public class HintNameEntryReader : IPEReader<HintNameEntry>
    {
        public HintNameEntry Read(IStructReader reader, IPE32PlusContext image)
        {
            var exportNameTableIndex = reader.Read<ushort>();
            var nameBuilder = new StringBuilder();
            char nextChar;
            while ((nextChar = (char)reader.Read<byte>()) != '\0')
                nameBuilder.Append(nextChar);
            return new HintNameEntry(exportNameTableIndex, nameBuilder.ToString());
        }
    }
}