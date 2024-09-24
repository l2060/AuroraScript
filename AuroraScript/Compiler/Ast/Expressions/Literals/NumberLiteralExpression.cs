using AuroraScript.Tokens;


namespace AuroraScript.Ast.Expressions.Literals
{
    internal class NumberLiteralExpression : ValueExpression<Double>
    {
        internal NumberLiteralExpression(ValueToken value) : base(value)
        {
            if (value.Value.StartsWith("0x"))
            {
                this.Value = Convert.ToUInt64(value.Value,16);
            }
            else
            {
                this.Value = Double.Parse(value.Value.Replace("_",""));
            }





        }
    }
}
