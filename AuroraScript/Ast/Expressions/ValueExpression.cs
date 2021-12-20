
namespace AuroraScript.Ast.Expressions
{
    public class ValueExpression: Expression
    {

        internal ValueExpression(Token value)
        {
            this.Value = value;
        }
        public Token Value { get; set; }

    }
}
