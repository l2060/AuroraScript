using AuroraScript.Compiler;

namespace AuroraScript.Ast.Expressions
{
    public abstract class OperatorExpression : Expression
    {
        internal OperatorExpression(Operator @operator)
        {
            this.Operator = @operator;
            this.operands = new List<Expression>();
            this.Precedence = this.Operator.Precedence;
            this.IsOperand = this.Operator.IsOperand;
        }

        private List<Expression> operands;

        /// <summary>
        /// operator precedence
        /// </summary>
        internal Int32 Precedence
        {
            get;
            private set;
        }

        /// <summary>
        /// this expression is Operand
        /// </summary>
        internal Boolean IsOperand
        {
            get;
            private set;
        }

        /// <summary>
        /// upgrade operator expression up up
        /// </summary>
        internal void Upgrade(Operator @operator)
        {
            this.Precedence = @operator.Precedence;
            this.IsOperand = @operator.IsOperand;
        }

        internal void Push(Expression expression)
        {
            operands.Add(expression);
        }

        /// <summary>
        /// Gets or sets the operator this expression refers to.
        /// </summary>
        public Operator Operator
        {
            get;
            private set;
        }
    }
}