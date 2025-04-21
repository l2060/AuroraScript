using AuroraScript.Ast.Expressions;

namespace AuroraScript.Compiler.Ast.Expressions
{
    /// <summary>
    /// 二元表达式
    /// </summary>
    public class InExpression : OperatorExpression
    {
        internal InExpression(Operator @operator) : base(@operator)
        {
        }

        //public Token Operator { get; set; }

        public Expression Left
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }

        public Expression Right
        {
            get
            {
                return this.childrens[1] as Expression;
            }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptInExpression(this);
        }


        public override string ToString()
        {
            var isPriority = false;
            if (this.Parent is BinaryExpression parent)
            {
                isPriority = parent.Operator.Precedence > this.Operator.Precedence;
            }
            var value = $"{Left} {Operator} {Right}";
            if (isPriority) return $"({value})";
            return value;
        }

    }
}