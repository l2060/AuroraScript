using AuroraScript.Tokens;

namespace AuroraScript.Ast.Expressions.Literals
{
    internal class NullLiteralExpression : ValueExpression<Object>
    {
        internal NullLiteralExpression(ValueToken value) : base(value)
        {
            this.Value = null;
        }
    }
}