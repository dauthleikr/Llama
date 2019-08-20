namespace Llama.PE.Idata
{
    using System.Text;
    using BinaryUtils;

    internal class HintNameEntryMapper : IPEMapper<HintNameEntry>
    {
        public void Write(HintNameEntry representation, IStructWriter writer)
        {
            writer.Write(representation.ExportNamePointerTableIndex);
            writer.WriteArray(Encoding.ASCII.GetBytes(representation.Name));
            var lastByteRva = writer.Write<byte>(0); // String zero termination

            // A trailing zero-pad byte appears after the trailing null byte, if necessary,
            // to align the next entry on an even boundary.
            if ((lastByteRva + 1) % 2 != 0)
                writer.Write<byte>(0);
        }

        public HintNameEntry Read(IStructReader reader)
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