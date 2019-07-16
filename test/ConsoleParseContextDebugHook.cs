namespace test
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Llama.Parser.Abstractions;
    using Llama.Parser.Framework;

    internal class ConsoleParseContextDebugHook : IStandardParseContextDebugHook
    {
        private struct ConsoleArea
        {
            public readonly int PosX, PosY;

            public ConsoleArea(int posX, int posY)
            {
                PosX = posX;
                PosY = posY;
            }

            public void SetToPos()
            {
                Console.CursorTop = PosY;
                Console.CursorLeft = PosX;
            }
        }

        private readonly Stack<Type> _entityTypes = new Stack<Type>();

        private readonly Stack<ConsoleArea> _printAreas = new Stack<ConsoleArea>();
        private int _level = -1;

        public ConsoleParseContextDebugHook() => Console.OutputEncoding = Encoding.UTF8;

        public void IncreaseLevel(Type tokenType)
        {
            _level++;
            _entityTypes.Push(tokenType);
        }

        public void DecreaseLevel()
        {
            _level--;
            _entityTypes.Pop();
            if (_printAreas.Count > _entityTypes.Count)
                _printAreas.Pop();
        }

        public void ParsingSkippedNotPlausible(ISourcePeeker source)
        {
            PrintLevelAndType(_entityTypes.Peek());
            PrintCodePreview(source);
            Write(" Not plausible\n", ConsoleColor.Yellow);
        }

        public void ParsingEnd<T>(IParseResult<T> result) where T : class, IEntity
        {
            var x = Console.CursorLeft;
            var y = Console.CursorTop;

            _printAreas.Pop().SetToPos();
            if (result.Successful)
            {
                Write(" succeeded", ConsoleColor.Green);
            }
            else
            {
                Write($" failed({result.ResultError.ConfidenceMetric}):", ConsoleColor.Red);
                Write($" {result.ResultError.Message}");
            }

            Console.CursorLeft = x;
            Console.CursorTop = y;
        }

        public void ParsingStart(ISourcePeeker source)
        {
            PrintLevelAndType(_entityTypes.Peek());
            PrintCodePreview(source);
            _printAreas.Push(new ConsoleArea(Console.CursorLeft, Console.CursorTop));
            Write("\n");
        }

        private static void PrintCodePreview(ISourcePeeker source)
        {
            Write(" |");
            PrintCodeChar(source.PeekFurther(-5), ConsoleColor.DarkMagenta);
            PrintCodeChar(source.PeekFurther(-4), ConsoleColor.DarkMagenta);
            PrintCodeChar(source.PeekFurther(-3), ConsoleColor.DarkMagenta);
            PrintCodeChar(source.PeekFurther(-2), ConsoleColor.DarkMagenta);
            PrintCodeChar(source.PeekFurther(-1), ConsoleColor.DarkMagenta);
            PrintCodeChar(source.Peek(), ConsoleColor.Magenta);
            Write("|");
        }

        private static void PrintCodeChar(char code, ConsoleColor color)
        {
            if (code == '\0')
                return;
            if (char.IsWhiteSpace(code) || char.IsControl(code))
                Write(new string(' ', 1), color);
            else
                Write(new string(code, 1), color);
        }

        private void PrintLevelAndType(Type tokenType)
        {
            for (var i = 0; i < _level * 2; i++)
                if (i % 2 == 0)
                    Write("|", ConsoleColor.White);
                else
                    Write(" ");
            var typeName = tokenType.Name;
            if (typeName.StartsWith('I') && typeName.Length > 1 && char.IsUpper(typeName[1]))
                typeName = typeName.Substring(1);
            if (typeName.EndsWith("Token"))
                typeName = typeName.Substring(0, typeName.Length - 5);
            Write(typeName, ConsoleColor.White);
        }

        private static void Write(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = oldColor;
        }
    }
}