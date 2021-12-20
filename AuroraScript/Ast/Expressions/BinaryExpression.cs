
namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 二元表达式
    /// </summary>
    internal class BinaryExpression : OperatorExpression
    {
        public BinaryExpression(Operator @operator) : base(@operator)
        {
        }

        //public Token Operator { get; set; }

        public Expression Left
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }


        public Expression Right
        {
            get
            {
                return this.childrens[1] as Expression;
            }
        }
    }
}
