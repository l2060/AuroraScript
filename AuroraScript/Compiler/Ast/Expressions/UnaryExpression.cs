using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    internal enum UnaryType
    {
        Prefix,
        Post
    }



    /// <summary>
    /// Postfix Expression
    /// i++
    /// i--
    /// </summary>
    internal class UnaryExpression : OperatorExpression
    {
        public UnaryType Type { get; private set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptUnaryExpression(this);
        }

        internal UnaryExpression(Operator @operator, UnaryType type) : base(@operator)
        {
            Type = type;
        }

        public Expression Operand
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }

        public override string ToString()
        {
            if (Type == UnaryType.Post)
            {
                return $"{Operand}{this.Operator}";
            }
            else
            {
                return $"{this.Operator}{Operand}";
            }
        }
    }

}
