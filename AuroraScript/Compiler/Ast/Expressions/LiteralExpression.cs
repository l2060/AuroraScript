using AuroraScript.Compiler;
using AuroraScript.Compiler.Exceptions;
using AuroraScript.Tokens;
using System;

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
                throw new AuroraParseException("无效的Token", token, "");
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