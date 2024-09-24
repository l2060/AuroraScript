using AuroraScript.Tokens;

namespace AuroraScript.Ast.Expressions.Literals
{
    internal class BooleanLiteralExpression : ValueExpression<Boolean>
    {

        internal BooleanLiteralExpression(ValueToken value) : base(value)
        {
            this.Value = Boolean.Parse(value.Value);
        }
    }
}
