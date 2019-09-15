namespace Llama.Parser
{
    using System;

    public interface IParse<out T>
    {
        T Read<TKindEnum>(IParseContext<TKindEnum> context) where TKindEnum : Enum;
    }
}