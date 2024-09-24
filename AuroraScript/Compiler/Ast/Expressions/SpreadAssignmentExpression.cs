using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuroraScript.Compiler;

namespace AuroraScript.Ast.Expressions
{
    internal class SpreadAssignmentExpression : PrefixUnaryExpression
    {
        internal SpreadAssignmentExpression() : base(Operator.PreSpread)
        {
        }



    }
}
