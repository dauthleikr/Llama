using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Parsers
{
    using Framework;
    using Tokens;

    class CommaParser : AtomicTokenParser<CommaToken>
    {
        protected override ITokenizationResult<CommaToken> TryReadTokenInternal(ISourceReader reader, IParseContext context)
        {
            reader.Eat();
            return new CommaToken();
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => reader.Peek() == ',';
    }
}
