

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 成员函数表达式
    /// </summary>
    internal class MemberAccessExpression : OperatorExpression
    {
        public MemberAccessExpression(Operator @operator) : base(@operator)
        {

        }




        /// <summary>
        /// super object name
        /// </summary>
        public AstNode Base { get; private set; }
    }

}
