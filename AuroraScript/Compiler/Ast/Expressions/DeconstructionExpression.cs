using AuroraScript.Compiler;

namespace AuroraScript.Ast.Expressions
{

    /// <summary>
    /// 解构运算符
    /// </summary>
    internal class DeconstructionExpression : PrefixUnaryExpression
    {
        internal DeconstructionExpression() : base(Operator.PreSpread)
        {
        }


        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptDeconstructionExpression(this);
        }
    }
}