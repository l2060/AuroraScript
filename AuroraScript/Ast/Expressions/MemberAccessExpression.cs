

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 成员函数表达式
    /// </summary>
    public class MemberAccessExpression : OperatorExpression
    {
        internal MemberAccessExpression(Operator @operator) : base(@operator)
        {

        }




        /// <summary>
        /// super object name
        /// </summary>
        public AstNode Base { get; private set; }
    }

}
