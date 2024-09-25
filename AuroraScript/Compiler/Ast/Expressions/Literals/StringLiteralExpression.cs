using AuroraScript.Tokens;

namespace AuroraScript.Ast.Expressions.Literals
{
    internal class StringLiteralExpression : ValueExpression<String>
    {
        internal StringLiteralExpression(ValueToken value) : base(value)
        {
            this.Value = value.Value;
        }
    }
}