using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    public enum UnaryType
    {
        Prefix,
        Post
    }



    /// <summary>
    /// Postfix Expression
    /// i++
    /// i--
    /// </summary>
    public class UnaryExpression : OperatorExpression
    {
        public UnaryType Type { get; private set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitUnaryExpression(this);
        }

        internal UnaryExpression(Operator @operator, UnaryType type) : base(@operator)
        {
            Type = type;
        }

        public Exception Operand { get; set; }

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
                return this.childrens[0] as Expression;
            }
        }


        public override string ToString()
        {
            if (Type == UnaryType.Post)
            {
                return $"{Left}{this.Operator}";
            }
            else
            {
                return $"{this.Operator}{Right}";
            }
        }




    }

}