namespace Llama.Parser.Nodes
{
    public class If : IStatement
    {
        public IStatement ElseInstruction { get; }
        public IExpression Condition { get; }
        public IStatement Instruction { get; }

        public If(IExpression condition, IStatement instruction, IStatement elseInstruction = null)
        {
            Condition = condition;
            Instruction = instruction;
            ElseInstruction = elseInstruction;
        }
    }
}