namespace Llama.Parser.Nodes
{
    public class Return : IStatement
    {
        public IExpression ReturnValue { get; }

        public Return(IExpression returnValue = null) => ReturnValue = returnValue;
    }
}