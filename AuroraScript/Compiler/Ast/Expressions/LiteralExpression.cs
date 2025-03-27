using AuroraScript.Compiler;
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
                this.Value = numberToken.NumberValue;
            }
            else if (token is NullToken)
            {
                this.Value = null;
            }
            else if (token is BooleanToken booleanToken)
            {
                this.Value = booleanToken.BoolValue;
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

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitLiteralExpression(this);
        }

        public override string ToString()
        {
            return Token.ToValue();
        }



    }
}