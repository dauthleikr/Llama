namespace Llama.Parser.Entities.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Abstractions;
    using Framework;

    public class FunctionCallEntity : EntityBase<FunctionCallEntity>
    {
        public readonly CloseParanthesisEntity CloseParanthesisEntity;
        public readonly IReadOnlyList<CommaEntity> Commas;
        public readonly OpenParanthesisEntity OpenParanthesisEntity;
        public readonly IReadOnlyList<IExpressionEntity> Parameters;

        public FunctionCallEntity(OpenParanthesisEntity open, IReadOnlyList<IExpressionEntity> parameters, IReadOnlyList<CommaEntity> commaTokens, CloseParanthesisEntity close)
        {
            OpenParanthesisEntity = open;
            Parameters = parameters;
            Commas = commaTokens;
            CloseParanthesisEntity = close;

            if (parameters.Count - 1 != commaTokens.Count && !(parameters.Count == 0 && commaTokens.Count == 0))
                throw new Exception($"{parameters.Count} parameters and {commaTokens.Count} commas in {nameof(FunctionCallEntity)}");
        }

        public override void WalkRecursive(ISourceWalker walker, bool codeChildrenOnly = true)
        {
            walker.Walk(this);
            OpenParanthesisEntity.WalkRecursive(walker, codeChildrenOnly);
            for (var index = 0; index < Parameters.Count; index++)
            {
                Parameters[index].WalkRecursive(walker, codeChildrenOnly);
                if (index < Commas.Count)
                    Commas[index].WalkRecursive(walker, codeChildrenOnly);
            }

            CloseParanthesisEntity.WalkRecursive(walker, codeChildrenOnly);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(OpenParanthesisEntity);
            for (var i = 0; i < Parameters.Count; i++)
            {
                builder.Append(Parameters[i]);
                if (Commas.Count > i)
                    builder.Append(Commas[i]);
            }

            builder.Append(CloseParanthesisEntity);
            return builder.ToString();
        }
    }
}