using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Parsers
{
    using Abstractions;
    using Entities;
    using Framework;

    class AssignmentParser : AtomicEntityParser<AssignmentEntity>
    {
        public override bool IsPlausible(ISourcePeeker reader, IParseContext context) => reader.Peek() == '=';

        protected override IParseResult<AssignmentEntity> TryReadEntityInternal(ISourceReader reader, IParseContext context)
        {
            reader.Eat();
            return new AssignmentEntity();
        }
    }
}
