using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Compiler.Ast.Statements
{

    public class YieldStatement : Statement
    {
        internal YieldStatement()
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitYieldExpression(this);
        }
    }
}