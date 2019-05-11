using System.Collections.Generic;

namespace llama.syntax
{
    public static class SourceReaderExtensions
    {
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

        public static bool TryRead(this ISourceReader reader, string text)
        {
            for (var i = 0; i < text.Length; i++)
                if (reader.PeekFurther(i) != text[i])
                    return false;
            reader.Eat(text.Length);
            return true;
        }
    }
}
