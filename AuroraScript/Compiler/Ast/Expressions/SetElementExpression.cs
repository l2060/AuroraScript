using AuroraScript.Ast.Expressions;

namespace AuroraScript.Compiler.Ast.Expressions
{
    public class SetElementExpression : OperatorExpression
    {
        public SetElementExpression() : base(Operator.Assignment)
        {
        }


        public Expression Object
        {
            get
            {
                return this.childrens[1] as Expression;
            }
        }


        public Expression Index
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }


        public Expression Value
        {
            get
            {
                return this.childrens[2] as Expression;
            }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitSetElementExpression(this);
        }

        public override string ToString()
        {
            return $"{Object}[{Index}] = {Value}";
        }



    }
}
