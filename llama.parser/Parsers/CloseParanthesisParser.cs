using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Parsers
{
    using Framework;
    using Tokens;

    class CloseParanthesisParser : AtomicTokenParser<CloseParanthesisToken>
    {
        protected override ITokenizationResult<CloseParanthesisToken> TryReadTokenInternal(ISourceReader reader, IParseContext context)
        {
            reader.Eat();
            return new CloseParanthesisToken();
        }

        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => reader.Peek() == ')';
    }
}
