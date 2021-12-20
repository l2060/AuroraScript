

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 前缀表达式
    /// </summary>
    internal class PrefixUnaryExpression : OperatorExpression
    {
        public PrefixUnaryExpression(Operator @operator) : base(@operator)
        {
        }


        /// <summary>
        /// -5
        /// !name
        /// ++n
        /// --n
        /// </summary>
        //public Token Operator { get; set; }

        public Exception Operand { get; set; }

    }
}
