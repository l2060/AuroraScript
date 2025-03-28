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

        

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitCompoundExpression(this);
        }
    }
}
