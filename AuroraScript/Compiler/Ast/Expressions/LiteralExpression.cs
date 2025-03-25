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

            if (token is NumberToken numberToken)
            {
                if (numberToken.Type == Tokens.ValueType.DoubleNumber)
                {
                    this.Value = numberToken.DoubleValue;
                }
                else
                {
                    this.Value = numberToken.IntegerValue;
                }
            }
            else if (token is NullToken)
            {
                this.Value = null;
            }
            else if (token is BooleanToken booleanToken)
            {
                this.Value = Boolean.Parse(token.Value);
            }
            else if (token is StringToken stringToken)
            {
                this.Value = stringToken.Value;
            }
            else
            {
                throw new Exception("无效的Token");
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