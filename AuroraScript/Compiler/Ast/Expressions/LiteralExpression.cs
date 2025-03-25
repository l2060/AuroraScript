using AuroraScript.Compiler;
using AuroraScript.Stream;
using AuroraScript.Tokens;

namespace AuroraScript.Ast.Expressions
{
    public class LiteralExpression : Expression
    {
        internal LiteralExpression(ValueToken token)
        {
            this.Token = token;

            if (token.Type == Tokens.ValueType.Null)
            {
                this.Value = null;
            }
            else if (token.Type == Tokens.ValueType.Number)
            {
                if (token.Value.StartsWith("0x"))
                {
                    this.Value = Convert.ToUInt64(token.Value, 16);
                }
                else
                {
                    this.Value = Double.Parse(token.Value.Replace("_", ""));
                }
            }
            else if (token.Type == Tokens.ValueType.String)
            {
                this.Value = token.Value;
            }
            else if (token.Type == Tokens.ValueType.Boolean)
            {
                this.Value = Boolean.Parse(token.Value);
            }
        }

        public ValueToken Token { get; set; }

        public Object Value { get; protected set; }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Token.ToValue());
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitLiteralExpression(this);
        }
    }
}