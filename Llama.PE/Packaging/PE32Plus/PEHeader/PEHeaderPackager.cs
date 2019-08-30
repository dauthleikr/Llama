namespace Llama.PE.Packaging.PE32Plus.PEHeader
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using BinaryUtils;
    using Structures.Header;

    internal class PEHeaderPackager : IPackage<IPEInfo, IPEResult>
    {
        public IPEResult Package(IPEInfo param)
        {
            var peHeader = new PEHeader
            {
                Characteristics = param.Characteristics,
                Architecture = param.Architecture,
                CreationTimePOSIX = (uint)((DateTimeOffset)param.TimeStamp).ToUnixTimeSeconds(),
                NumberOfSections = param.NumberOfSections,
                Magic = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("PE\0\0")),
                OptionalHeaderSize = (ushort)Marshal.SizeOf<PE32PlusOptionalHeader>()
            };
            return new PEResult(StructConverter.GetBytes(peHeader));
        }
    }
}