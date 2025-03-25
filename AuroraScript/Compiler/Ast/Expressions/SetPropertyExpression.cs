using AuroraScript.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Compiler.Ast.Expressions
{
    public class SetPropertyExpression : OperatorExpression
    {
        public SetPropertyExpression() : base( Operator.Assignment)
        {
        }


        public Expression Object
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }


        public Expression Property
        {
            get
            {
                return this.childrens[1] as Expression;
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
            visitor.VisitSetPropertyExpression(this);
        }
    }
}
