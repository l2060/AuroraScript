

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 前缀表达式
    /// </summary>
    public class PrefixUnaryExpression : OperatorExpression
    {
        internal PrefixUnaryExpression(Operator @operator) : base(@operator)
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
