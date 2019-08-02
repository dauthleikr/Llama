namespace test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;
    using Llama.Parser.Entities;
    using Llama.Parser.Framework;
    using Llama.Parser.NonCode;
    using Llama.Parser.NonCode.Parsers;

    internal class Program
    {
        class Huh : PEBuilder
        {
            public Huh(PEHeaderBuilder header, Func<IEnumerable<Blob>, BlobContentId> deterministicIdProvider) : base(header, deterministicIdProvider)
            {
            }

            protected override ImmutableArray<Section> CreateSections()
            {
                var sec = new Section[1];
                sec[0] = new Section(".text", SectionCharacteristics.Align512Bytes | SectionCharacteristics.MemExecute | SectionCharacteristics.MemRead | SectionCharacteristics.ContainsCode);
                return sec.ToImmutableArray();
            }

            protected override PEDirectoriesBuilder GetDirectories() => new PEDirectoriesBuilder();

            protected override BlobBuilder SerializeSection(string name, SectionLocation location)
            {
                Console.WriteLine(name);
                var blobBuilder = new BlobBuilder();
                for (var i = 0; i < 10; i++)
                {
                    blobBuilder.WriteByte(0xC3);
                }
                return blobBuilder;
            }
        }

        private static void Main(string[] args)
        {

            PEHeaderBuilder hmm = new PEHeaderBuilder(Machine.Amd64, imageCharacteristics: Characteristics.ExecutableImage);
            
            PEBuilder wow = new Huh(hmm, null);

            BlobBuilder code = new BlobBuilder();

            var blobContentId = wow.Serialize(code);

            File.WriteAllBytes("lol.exe", code.ToArray());

            return;
            //  Trace.Listeners.Add(new ConsoleTrace());

            var reader = new StringSourceReader("lol = a.b * c + 12_3.1f - h(a, 2 << 3, f(xd))");
            var errorMgr = new StandardErrorManager(new DebugErrorEscalator());
            var parsers = new ParseStrategies();

            var nonCodeParsers = new NonCodeParser(new IParseNonCode[]
            {
                new BlockCommentParser(),
                new LineCommentParser(),
                new WhitespaceParser()
            });
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