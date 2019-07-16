namespace Llama.Parser.Framework
{
    using System.Collections.Generic;
    using Abstractions;

    internal static class SourceReaderExtensions
    {
        internal static void Vomit(this ISourceReader reader, int num = 1) => reader.Eat(-num);

        public static string TryReadLongest(this ISourceReader reader, HashSet<string> options, int maxOptionLength)
        {
            var offset = 0;
            string lastValid = null;
            var current = string.Empty;
            while (!reader.IsAtEnd && offset < maxOptionLength)
            {
                current += reader.PeekFurther(offset++);
                if (options.Contains(current))
                    lastValid = current;
            }

            if (lastValid == null)
                return null;

            reader.Eat(lastValid.Length);
            return lastValid;
        }
    }
}