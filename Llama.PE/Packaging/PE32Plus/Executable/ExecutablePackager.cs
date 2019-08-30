namespace Llama.PE.Packaging.PE32Plus.Executable
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection.PortableExecutable;
    using System.Runtime.InteropServices;
    using MZHeader;
    using OptionalHeader;
    using PEHeader;
    using Sections;
    using Structures.Header;
    using PEHeader = Structures.Header.PEHeader;

    internal class ExecutablePackager : IPackage<IExectuableInfo, IPackagingResult>
    {
        private readonly IPackage<IMZInfo, IMZResult> _mzHeaderPackager;
        private readonly IPackage<IOptionalHeaderInfo, IOptionalHeaderResult> _optHeaderPackager;
        private readonly IPackage<IPEInfo, IPEResult> _peHeaderPackager;
        private readonly IPackage<ISectionHeadersInfo, ISectionsResult> _sectionsPackager;

        public ExecutablePackager(
            IPackage<IMZInfo, IMZResult> mzHeaderPackager,
            IPackage<IPEInfo, IPEResult> peHeaderPackager,
            IPackage<IOptionalHeaderInfo, IOptionalHeaderResult> optHeaderPackager,
            IPackage<ISectionHeadersInfo, ISectionsResult> sectionsPackager
        )
        {
            _mzHeaderPackager = mzHeaderPackager ?? throw new ArgumentNullException(nameof(mzHeaderPackager));
            _peHeaderPackager = peHeaderPackager ?? throw new ArgumentNullException(nameof(peHeaderPackager));
            _optHeaderPackager = optHeaderPackager ?? throw new ArgumentNullException(nameof(optHeaderPackager));
            _sectionsPackager = sectionsPackager ?? throw new ArgumentNullException(nameof(sectionsPackager));
        }

        public IPackagingResult Package(IExectuableInfo param)
        {
            var rawData = new MemoryStream();
            var mzResult = _mzHeaderPackager.Package(null);
            var sectionsResult = _sectionsPackager.Package(MakeSectionHeadersInfo(param, mzResult));
            var optHeaderResult = _optHeaderPackager.Package(MakeOptionalHeaderInfo(param, mzResult, sectionsResult));
            var peHeaderResult = _peHeaderPackager.Package(MakePEHeaderInfo(param, optHeaderResult, sectionsResult));

            rawData.Write(mzResult.RawData);
            rawData.Position = mzResult.NewHeaderOffset;
            rawData.Write(peHeaderResult.RawData);
            rawData.Write(optHeaderResult.RawData);
            rawData.Write(sectionsResult.RawData);
            return new ExecutableResult(rawData.ToArray());
        }

        private static ISectionHeadersInfo MakeSectionHeadersInfo(IExectuableInfo exeInfo, IMZResult mzResult) =>
            new SectionHeadersInfo(
                exeInfo.OtherSections,
                exeInfo.TextSection,
                exeInfo.Imports,
                exeInfo.FileAlignment,
                exeInfo.SectionAlignment,
                (uint)(mzResult.NewHeaderOffset + Marshal.SizeOf<PEHeader>() + Marshal.SizeOf<PE32PlusOptionalHeader>())
            );

        private static IOptionalHeaderInfo MakeOptionalHeaderInfo(IExectuableInfo exeInfo, IMZResult mzResult, ISectionsResult sectionsResult) =>
            new OptionalHeaderInfo(
                mzResult,
                sectionsResult,
                exeInfo.MajorLinkerVersion,
                exeInfo.MinorLinkerVersion,
                exeInfo.MajorOperatingSystemVersion,
                exeInfo.MinorOperatingSystemVersion,
                exeInfo.MajorSubSystemVersion,
                exeInfo.MinorSubSystemVersion,
                exeInfo.MajorImageVersion,
                exeInfo.MinorImageVersion,
                exeInfo.FileAlignment,
                exeInfo.SectionAlignment,
                exeInfo.Subsystem,
                exeInfo.DllCharacteristics,
                exeInfo.StackSizeReserve,
                exeInfo.StackSizeCommit,
                exeInfo.HeapSizeReserve,
                exeInfo.HeapSizeCommit,
                exeInfo.ImageBase
            );

        [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
        private static IPEInfo MakePEHeaderInfo(IExectuableInfo exeInfo, IOptionalHeaderResult optHeaderResult, ISectionsResult sectionsResult)
        {
            var characteristics = Characteristics.ExecutableImage | Characteristics.LargeAddressAware;
            if (!optHeaderResult.HasDebugInfo)
                characteristics |= Characteristics.DebugStripped;
            if (!optHeaderResult.HasRelocationInfo)
                characteristics |= Characteristics.RelocsStripped;
            if (!exeInfo.MayRunFromNetwork)
                characteristics |= Characteristics.NetRunFromSwap;
            if (!exeInfo.MayRunFromRemoveableDrive)
                characteristics |= Characteristics.RemovableRunFromSwap;
            return new PEHeaderInfo(characteristics, Machine.Amd64, DateTime.Now, (ushort)sectionsResult.SectionHeaders.Count);
        }
    }
}