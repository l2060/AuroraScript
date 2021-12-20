

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 函数调用
    /// </summary>
    public class CallExpression: OperatorExpression
    {
        internal CallExpression(Operator @operator) : base(@operator)
        {
            this.Arguments = new List<Expression>();
        }

        public List<Expression> Arguments { get; set; }
        public Expression Base { get; set; }


    }
}
