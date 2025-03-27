using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    public class CastTypeExpression : PrefixUnaryExpression
    {
        internal CastTypeExpression(Operator @operator) : base(@operator)
        {
        }

        public Expression Typed { get; set; }

        public override void Accept(IAstVisitor visitor)
        {

        }
    }
}