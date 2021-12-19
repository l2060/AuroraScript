using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    internal class GroupExpression : OperatorExpression
    {
        public GroupExpression(Operator @operator) : base(@operator)
        {
        }
    }
}
