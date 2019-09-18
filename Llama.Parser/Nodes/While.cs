namespace Llama.Parser.Nodes
{
    public class While : IStatement
    {
        public IExpression Condition { get; }
        public IStatement Instruction { get; }

        public While(IExpression condition, IStatement instruction)
        {
            Condition = condition;
            Instruction = instruction;
        }
    }
}