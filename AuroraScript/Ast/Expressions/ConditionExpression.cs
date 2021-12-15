using AuroraScript.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    internal class ConditionExpression : Expression
    {
        public Expression Condition { get; set; }

        public BlockStatement Body { get; set; }


    }
}
