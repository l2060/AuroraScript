using AuroraScript.Compiler;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 二元表达式
    /// </summary>
    public class BinaryExpression : OperatorExpression
    {
        internal BinaryExpression(Operator @operator) : base(@operator)
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
            visitor.AcceptBinaryExpression(this);
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