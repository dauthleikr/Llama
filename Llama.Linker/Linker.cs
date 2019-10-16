namespace Llama.Linker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.PortableExecutable;
    using BinaryUtils;
    using Compiler;
    using PE.Builder.PE32Plus;

    public class Linker : IAddressFixer
    {
        private const int ForcedDataAlignment = 8;
        private readonly List<ConstDataFixup> _constDataAddressFixes = new List<ConstDataFixup>();
        private readonly List<ConstDataFixup> _constDataOffsetFixes = new List<ConstDataFixup>();
        private readonly Dictionary<string, Tuple<int, List<long>>> _dataOffsetFixes = new Dictionary<string, Tuple<int, List<long>>>();
        private readonly List<FunctionFixup> _functionAddressFixes = new List<FunctionFixup>();
        private readonly List<FunctionFixup> _functionOffsetFixes = new List<FunctionFixup>();
        private readonly List<IATFixup> _iatOffsetFixes = new List<IATFixup>();

        private readonly List<FunctionFixup> _resolvedFunctionFixes = new List<FunctionFixup>();

        public void FixIATEntryOffset(long position, string library, string function)
        {
            _iatOffsetFixes.Add(new IATFixup(position, library, function));
        }

        public void FixConstantDataOffset(long position, byte[] data)
        {
            _constDataOffsetFixes.Add(new ConstDataFixup(position, data));
        }

        public void FixConstantDataAddress(long position, byte[] data)
        {
            _constDataAddressFixes.Add(new ConstDataFixup(position, data));
        }

        public void FixDataOffset(long position, string identifier, int length = 8)
        {
            if (!_dataOffsetFixes.TryGetValue(identifier, out var value))
                _dataOffsetFixes[identifier] = value = new Tuple<int, List<long>>(length, new List<long>());

            value.Item2.Add(position);
        }

        public void FixFunctionOffset(long position, string identifier)
        {
            _functionOffsetFixes.Add(new FunctionFixup(position, identifier));
        }

        public void FixFunctionAddress(long position, string identifier)
        {
            _functionAddressFixes.Add(new FunctionFixup(position, identifier));
        }

        public void ResolveFunctionFixes(string identifier, long position)
        {
            _resolvedFunctionFixes.Add(new FunctionFixup(position, identifier));
        }

        public void Insert(long position, int count)
        {
            static void ProcessInsertForFixups(IEnumerable<IFixupInfo> fixupsEnumerable, long pos, int count)
            {
                foreach (var fixupInfo in fixupsEnumerable)
                {
                    if (fixupInfo.Position >= pos)
                        fixupInfo.Position += count;
                }
            }

            ProcessInsertForFixups(_constDataAddressFixes, position, count);
            ProcessInsertForFixups(_constDataOffsetFixes, position, count);
            ProcessInsertForFixups(_functionAddressFixes, position, count);
            ProcessInsertForFixups(_functionOffsetFixes, position, count);
            ProcessInsertForFixups(_iatOffsetFixes, position, count);
            ProcessInsertForFixups(_resolvedFunctionFixes, position, count);

            foreach (var fixupList in _dataOffsetFixes.Values.Select(value => value.Item2))
            {
                for (var i = 0; i < fixupList.Count; i++)
                {
                    if (fixupList[i] >= position)
                        fixupList[i] += count;
                }
            }
        }

        public void CopyTo(IAddressFixer other, long offset)
        {
            foreach (var fixup in _constDataOffsetFixes)
                other.FixConstantDataOffset(fixup.Position, fixup.ConstData);
            foreach (var fixup in _constDataAddressFixes)
                other.FixConstantDataAddress(fixup.Position, fixup.ConstData);
            foreach (var fixup in _iatOffsetFixes)
                other.FixIATEntryOffset(fixup.Position, fixup.Library, fixup.Function);
            foreach (var fixup in _functionAddressFixes)
                other.FixFunctionAddress(fixup.Position, fixup.Function);
            foreach (var fixup in _functionOffsetFixes)
                other.FixFunctionOffset(fixup.Position, fixup.Function);
            foreach (var fixup in _resolvedFunctionFixes)
                other.ResolveFunctionFixes(fixup.Function, fixup.Position);
            foreach (var (identifier, (length, positions)) in _dataOffsetFixes)
            {
                foreach (var fixupPosition in positions)
                    other.FixDataOffset(fixupPosition, identifier, length);
            }
        }

        public IEnumerable<long> GetCodeRelocationOffsets() =>
            _functionAddressFixes.Select(item => item.Position).Concat(_constDataAddressFixes.Select(item => item.Position));

        public long GetCodeOffsetOfKnownFunction(string identifier)
        {
            foreach (var resolvedFunctionFix in _resolvedFunctionFixes)
            {
                if (resolvedFunctionFix.Function == identifier)
                    return resolvedFunctionFix.Position;
            }

            return -1;
        }

        private IEnumerable<(string library, string function)> GetAllImports() =>
            _iatOffsetFixes.Select(item => (item.Library, item.Function)).Distinct();

        private int GetRdataSectionSize() =>
            _constDataOffsetFixes.Concat(_constDataAddressFixes).Sum(item => Round.Up(item.ConstData.Length, ForcedDataAlignment));

        private int GetDataSectionSize() => _dataOffsetFixes.Sum(item => Round.Up(item.Value.Item1, ForcedDataAlignment));

        public void LinkPreBuild(IExecutableBuilder builder)
        {
            foreach (var (library, function) in GetAllImports())
                builder.ImportFunction(library, function);

            var neededRdataSize = GetRdataSectionSize();
            if (neededRdataSize > 0)
            {
                builder.AddAdditionalSection(
                    ".rdata",
                    (uint)neededRdataSize,
                    SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.MemRead
                );
            }

            var neededDataSize = GetDataSectionSize();
            if (neededDataSize > 0)
            {
                builder.AddAdditionalSection(
                    ".data",
                    (uint)neededDataSize,
                    SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.MemRead | SectionCharacteristics.MemWrite
                );
            }

            foreach (var codeRelocationOffset in GetCodeRelocationOffsets())
                builder.AddRelocation64(".text", (uint)codeRelocationOffset);
        }

        public void LinkPostBuild(IExecutableBuilderResult peResult)
        {
            var codeBuffer = peResult.GetCodeSectionBuffer();
            var codeRVA = (long)peResult.GetSectionRVA(".text");

            var rdataBuffer = peResult.GetSectionBuffer(".rdata");
            var rdataOffset = peResult.GetSectionOffsetFromStartOfCode(".rdata");
            var rdataRVA = (long)peResult.GetSectionRVA(".rdata");
            var rdataPtr = 0;

            var dataOffset = peResult.GetSectionOffsetFromStartOfCode(".data");
            var dataPtr = 0;

            foreach (var iatOffsetFix in _iatOffsetFixes) // Fix references to IAT functions
            {
                var functionOffsetCode = peResult.GetIATEntryOffsetToStartOfCode(iatOffsetFix.Library, iatOffsetFix.Function);
                var rva = (int)(functionOffsetCode - iatOffsetFix.Position);
                BitConverter.GetBytes(rva).CopyTo(codeBuffer.Slice((int)iatOffsetFix.Position - 4, 4));
            }

            foreach (var constDataFix in _constDataOffsetFixes)
            {
                var offset = (int)(rdataOffset - constDataFix.Position + rdataPtr);
                BitConverter.GetBytes(offset).CopyTo(codeBuffer.Slice((int)constDataFix.Position - 4, 4));
                constDataFix.ConstData.CopyTo(rdataBuffer.Slice(rdataPtr));
                rdataPtr = Round.Up(rdataPtr + constDataFix.ConstData.Length, ForcedDataAlignment);
            }

            foreach (var constDataFix in _constDataAddressFixes)
            {
                var rva = rdataRVA + rdataPtr;
                BitConverter.GetBytes(rva).CopyTo(codeBuffer.Slice((int)constDataFix.Position - 8, 8));
                constDataFix.ConstData.CopyTo(rdataBuffer.Slice(rdataPtr));
                rdataPtr = Round.Up(rdataPtr + constDataFix.ConstData.Length, ForcedDataAlignment);
            }

            foreach (var functionOffsetFix in _functionOffsetFixes)
            {
                var codeOffsetOfKnownFunction = GetCodeOffsetOfKnownFunction(functionOffsetFix.Function);
                if (codeOffsetOfKnownFunction < 0)
                    throw new LinkException($"Cannot link function: \"{functionOffsetFix.Function}\" (function not defined)");
                var offset = (int)(codeOffsetOfKnownFunction - functionOffsetFix.Position);
                BitConverter.GetBytes(offset).CopyTo(codeBuffer.Slice((int)functionOffsetFix.Position - 4, 4));
            }

            foreach (var functionAddressFix in _functionAddressFixes)
            {
                var codeOffsetOfKnownFunction = GetCodeOffsetOfKnownFunction(functionAddressFix.Function);
                if (codeOffsetOfKnownFunction < 0)
                    throw new LinkException($"Cannot link function: \"{functionAddressFix.Function}\" (function not defined)");
                var rva = codeRVA + codeOffsetOfKnownFunction;
                BitConverter.GetBytes(rva).CopyTo(codeBuffer.Slice((int)functionAddressFix.Position - 8, 8));
            }

            foreach (var (size, fixes) in _dataOffsetFixes.Values)
            {
                foreach (var fix in fixes)
                {
                    var offset = (int)(dataOffset - fix + dataPtr);
                    BitConverter.GetBytes(offset).CopyTo(codeBuffer.Slice((int)fix - 4, 4));
                }

                dataPtr = Round.Up(dataPtr + size, ForcedDataAlignment);
            }
        }
    }
}