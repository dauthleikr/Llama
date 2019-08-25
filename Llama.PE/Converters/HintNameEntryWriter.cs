namespace Llama.PE.Idata
{
    using System.Text;
    using BinaryUtils;

    public class HintNameEntryWriter : IPEWriter<HintNameEntry>
    {
        public void Write(IStructWriter writer, HintNameEntry item)
        {
            writer.Write(item.ExportNamePointerTableIndex);
            writer.WriteArray(Encoding.ASCII.GetBytes(item.Name));
            var lastByteOffset = writer.Write<byte>(0); // String zero termination

            // A trailing zero-pad byte appears after the trailing null byte, if necessary,
            // to align the next entry on an even boundary.
            if ((lastByteOffset + 1) % 2 != 0)
                writer.Write<byte>(0);
        }
    }
}