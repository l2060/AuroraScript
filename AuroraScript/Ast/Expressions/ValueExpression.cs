
namespace AuroraScript.Ast.Expressions
{
    internal class ValueExpression: Expression
    {

        public ValueExpression(Token value)
        {
            this.Value = value;
        }
        public Token Value { get; set; }

    }
}
