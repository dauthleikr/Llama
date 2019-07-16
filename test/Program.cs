namespace test
{
    using System;
    using System.Diagnostics;
    using Llama.Parser.Entities;
    using Llama.Parser.Framework;
    using Llama.Parser.NonCode;
    using Llama.Parser.NonCode.Parsers;

    internal class Program
    {
        private class ConsoleTrace : TraceListener
        {
            public override void Write(string message)
            {
                Console.Write(message);
            }

            public override void WriteLine(string message)
            {
                Console.WriteLine(message);
            }
        }

        private static void Main(string[] args)
        {
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