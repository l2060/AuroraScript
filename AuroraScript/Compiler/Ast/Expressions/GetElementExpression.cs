using AuroraScript.Compiler;

namespace AuroraScript.Ast.Expressions
{
    public class GetElementExpression : OperatorExpression
    {
        internal GetElementExpression(Operator @operator) : base(@operator)
        {
        }

        public Expression Index
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }


        public Expression Object
        {
            get
            {
                return this.childrens[1] as Expression;
            }
        }


        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitGetElementExpression(this);
        }


        public override string ToString()
        {
            return $"{Object}[{Index}]";
        }

    }
}