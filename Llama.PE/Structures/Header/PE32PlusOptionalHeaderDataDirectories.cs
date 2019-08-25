namespace Llama.PE.Header
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///     The data directories part of the <see cref="PE32PlusOptionalHeader" /> header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PE32PlusOptionalHeaderDataDirectories
    {
        public ImageDataDirectory ExportTable;
        public ImageDataDirectory ImportTable;
        public ImageDataDirectory ResourceTable;
        public ImageDataDirectory ExceptionTable;
        public ImageDataDirectory CertificateTable;
        public ImageDataDirectory BaseRelocationTable;

        /// <summary>
        ///     Optional
        /// </summary>
        public ImageDataDirectory Debug;

        /// <summary>
        ///     Reserved, must be 0
        /// </summary>
        public ImageDataDirectory Architecture;

        /// <summary>
        ///     RVA of the value to be stored in the global pointer register. Size must be 0!
        /// </summary>
        public ImageDataDirectory GlobalPtr;

        public ImageDataDirectory TLSTable;
        public ImageDataDirectory LoadConfigTable;
        public ImageDataDirectory BoundImportTable;
        public ImageDataDirectory IAT;
        public ImageDataDirectory DelayImportDescriptor;
        public ImageDataDirectory CLRRuntimeHeader;

        /// <summary>
        ///     Must be 0!
        /// </summary>
        public ImageDataDirectory Reserved;
    }
}