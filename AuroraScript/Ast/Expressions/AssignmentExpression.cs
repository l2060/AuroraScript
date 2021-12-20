
namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 赋值
    /// </summary>
    public class AssignmentExpression : OperatorExpression
    {
        internal AssignmentExpression(Operator @operator) : base(@operator)
        {
        }

        public List<Token> Variables { get; set; }

        public List<AstNode> Inits { get; set; }



    }
}
