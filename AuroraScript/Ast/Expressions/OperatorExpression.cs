

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
        /// 操作符优先级
        /// </summary>
        internal int Precedence
        {
            get;
            private set;
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
