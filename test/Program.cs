namespace test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using Llama.Parser.Entities;
    using Llama.Parser.Framework;
    using Llama.Parser.NonCode;
    using Llama.Parser.NonCode.Parsers;
    using Llama.PE.Builder.PE32Plus;

    internal class Program
    {
        private class Huh : PEBuilder
        {
            public Huh(PEHeaderBuilder header, Func<IEnumerable<Blob>, BlobContentId> deterministicIdProvider) :
                base(header, deterministicIdProvider) { }

            protected override ImmutableArray<Section> CreateSections()
            {
                var sec = new Section[1];
                sec[0] = new Section(
                    ".text",
                    SectionCharacteristics.Align512Bytes |
                    SectionCharacteristics.MemExecute |
                    SectionCharacteristics.MemRead |
                    SectionCharacteristics.ContainsCode
                );
                return sec.ToImmutableArray();
            }

            protected override PEDirectoriesBuilder GetDirectories() => new PEDirectoriesBuilder();

            protected override BlobBuilder SerializeSection(string name, SectionLocation location)
            {
                Console.WriteLine(name);
                var blobBuilder = new BlobBuilder();
                for (var i = 0; i < 10; i++)
                    blobBuilder.WriteByte(0xC3);
                return blobBuilder;
            }
        }

        private static void CreateMsgBoxApp(FileInfo file)
        {
            var appBuilder = new ExecutableBuilder();
            var moduleBase = appBuilder.ImageBase;
            var buildResult = appBuilder
                .AddAdditionalSection(".data", 100, SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.MemRead)
                .ImportFunction("user32.dll", "MessageBoxA")
                .AddRelocation64(".text", 15)
                .Build(1000, 0);

            var dataOffsetToCode = buildResult.GetSectionOffsetFromStartOfCode(".data");
            var codeRVA = buildResult.GetSectionRVA(".text");
            var codeBuffer = buildResult.GetSectionBuffer(".text");
            var dataBuffer = buildResult.GetSectionBuffer(".data");
            Encoding.ASCII.GetBytes("Test").CopyTo(dataBuffer);

            var shellcode = new byte[]
                {
                    0x48, // mov rbp, rsp
                    0x89,
                    0xE5,
                    0x48, // sub rsp, 0x28
                    0x83,
                    0xEC,
                    0x28,
                    0x48, // xor rcx, rcx
                    0x31,
                    0xC9,
                    0x4D, // xor r9, r9
                    0x31,
                    0xC9,
                    0x48, // mov rdx, ...
                    0xBA
                }.Concat(BitConverter.GetBytes((ulong)((long)(moduleBase + codeRVA) + dataOffsetToCode)))
                .Concat(
                    new byte[]
                    {
                        0x49, // mov r8, rdx
                        0x89,
                        0xD0,
                        0xFF, // call [...]
                        0x15
                    }
                )
                .ToArray();
            shellcode = shellcode.Concat(
                    BitConverter.GetBytes((int)buildResult.GetIATEntryOffsetToStartOfCode("user32.dll", "MessageBoxA") - shellcode.Length - 4)
                )
                .Concat(
                    new byte[]
                    {
                        0xC3
                    }
                )
                .ToArray();
            shellcode.CopyTo(codeBuffer);
            using (var exeStream = file.Create())
                buildResult.Finish(exeStream);
        }

        private static void Main(string[] args)
        {
            CreateMsgBoxApp(new FileInfo("xdee.exe"));
            return;
            var hmm = new PEHeaderBuilder(Machine.Amd64, imageCharacteristics: Characteristics.ExecutableImage);

            PEBuilder wow = new Huh(hmm, null);

            var code = new BlobBuilder();

            var blobContentId = wow.Serialize(code);

            File.WriteAllBytes("lol.exe", code.ToArray());

            return;
            //  Trace.Listeners.Add(new ConsoleTrace());

            var reader = new StringSourceReader("lol = a.b * c + 12_3.1f - h(a, 2 << 3, f(xd))");
            var errorMgr = new StandardErrorManager(new DebugErrorEscalator());
            var parsers = new ParseStrategies();

            var nonCodeParsers = new NonCodeParser(
                new IParseNonCode[]
                {
                    new BlockCommentParser(),
                    new LineCommentParser(),
                    new WhitespaceParser()
                }
            );
            var context = new StandardParseContext(reader, errorMgr, new PanicResolvers(), parsers, nonCodeParsers);
            context.AddDebugHook(new ConsoleParseContextDebugHook());

            var time = Stopwatch.StartNew();
            if (context.TryRead(out IExpressionEntity result))
                Console.WriteLine($"{time.Elapsed.TotalMilliseconds:F4} ms");
            else
                Debug.WriteLine("Failed to parse");
            Console.ReadLine();
        }
    }
}