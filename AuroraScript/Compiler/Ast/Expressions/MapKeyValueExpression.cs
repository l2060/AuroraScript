using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    public class MapKeyValueExpression : BinaryExpression
    {
        internal MapKeyValueExpression(Token key, AstNode value) : base(Operator.SetMember)
        {
            this.Key = key;
            this.Value = value;
            value.Parent = this;
        }


        public Token Key { get; set; }
        public AstNode Value { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
            //visitor.VisitSetPropertyExpression(this);
        }


        public override string ToString()
        {
            if (Key != null)
            {
                return $"{Key.Value}: {Value}";
            }
            else
            {
                return Value.ToString();
            }
        }
    }
}