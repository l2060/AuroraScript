

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// Postfix Expression
    /// i++
    /// i--
    /// </summary>
    public class PostfixExpression : OperatorExpression
    {
        internal PostfixExpression(Operator @operator) : base(@operator)
        {
        }

        public Exception Operand { get; set; }

        public Expression Left
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }

        public override String ToString()
        {
            return $"({this.Left}{this.Operator.Symbol.Name})";
        }
    }
}
