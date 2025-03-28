using AuroraScript.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Compiler.Ast.Expressions
{
    public class CompoundExpression : OperatorExpression
    {
        public CompoundExpression(Operator @operator) : base(@operator)
        {

        }

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
            visitor.VisitCompoundExpression(this);
        }
    }
}
