using AuroraScript.Stream;
using AuroraScript.Tokens;

namespace AuroraScript.Ast.Expressions
{
    public class ValueExpression<T> : Expression
    {
        internal ValueExpression(ValueToken token)
        {
            this.Token = token;
        }

        public ValueToken Token { get; set; }

        public T Value { get; protected set; }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Token.ToValue());
        }
    }
}