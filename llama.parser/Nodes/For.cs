namespace Llama.Parser.Nodes
{
    internal class For : IStatement
    {
        public IStatement Instruction { get; }
        public Declaration Variable { get; }
        public IExpression Condition { get; }
        public IExpression Increment { get; }

        public For(IStatement instruction, Declaration variable = null, IExpression condition = null, IExpression increment = null)
        {
            Instruction = instruction;
            Variable = variable;
            Condition = condition;
            Increment = increment;
        }
    }
}