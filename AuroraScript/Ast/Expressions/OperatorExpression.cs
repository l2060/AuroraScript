

namespace AuroraScript.Ast.Expressions
{
    internal class OperatorExpression : Expression
    {
        public OperatorExpression(Operator @operator)
        {
            this.Operator = @operator;
            this.operands = new List<Expression>();
            this.Precedence = this.Operator.Precedence;
        }

        private List<Expression> operands;






        /// <summary>
        /// 操作符优先级
        /// </summary>
        public int Precedence
        {
            get;
            private set;
        }


        public void Push(Expression expression)
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
