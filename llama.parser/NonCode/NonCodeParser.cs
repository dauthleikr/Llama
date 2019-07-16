namespace Llama.Parser.NonCode
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using Entities;

    public class NonCodeParser : INonCodeParser
    {
        private readonly IParseNonCode[] _nonCodeParsers;
        private readonly Dictionary<long, int> _nonCodePositions = new Dictionary<long, int>();

        public NonCodeParser(IEnumerable<IParseNonCode> nonCodeParsers) => _nonCodeParsers = nonCodeParsers.ToArray();

        public INonCode ReadOrNull(ISourceReader reader)
        {
            var nonCode = ReadOneOrNull(reader);
            var nonCode2 = nonCode == null ? null : ReadOneOrNull(reader);
            if (nonCode2 != null)
            {
                var tokens = new List<INonCode> {nonCode, nonCode2};
                while (true)
                {
                    var token = ReadOneOrNull(reader);
                    if (token == null)
                        break;
                    tokens.Add(token);
                }

                return new AggregateNonCodeEntity(tokens.ToArray());
            }

            return nonCode;
        }

        public void MarkAsNonCode(long position, int length)
        {
            _nonCodePositions[position] = length;
        }

        private INonCode ReadOneOrNull(ISourceReader reader)
        {
            var startPosition = reader.Position;
            if (_nonCodePositions.TryGetValue(reader.Position, out var nonCodeLength))
                return new InvalidSyntaxNonCodeEntity(reader.Read(nonCodeLength));
            foreach (var nonCodeParser in _nonCodeParsers)
            {
                if (nonCodeParser.TryParse(reader, out var result))
                    return result;
                reader.Position = startPosition;
            }

            return null;
        }
    }
}