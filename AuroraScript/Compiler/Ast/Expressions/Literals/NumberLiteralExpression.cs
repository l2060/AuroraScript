using AuroraScript.Tokens;


namespace AuroraScript.Ast.Expressions.Literals
{
    internal class NumberLiteralExpression : ValueExpression<Double>
    {
        internal NumberLiteralExpression(ValueToken value) : base(value)
        {
            this.Value = Double.Parse(value.Value);
        }
    }
}
