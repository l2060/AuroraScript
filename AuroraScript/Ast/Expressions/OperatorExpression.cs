

namespace AuroraScript.Ast.Expressions
{
    public class OperatorExpression : Expression
    {
        internal OperatorExpression(Operator @operator)
        {
            this.Operator = @operator;
            this.operands = new List<Expression>();
            this.Precedence = this.Operator.Precedence;
        }

        private List<Expression> operands;



        /// <summary>
        /// operator precedence
        /// </summary>
        internal int Precedence
        {
            get;
            private set;
        }

        /// <summary>
        /// upgrade operator precedence up up
        /// </summary>
        internal void UpgradePrecedence(Operator @operator)
        {
            this.Precedence = @operator.Precedence;
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
