using AuroraScript.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Statements
{
    internal class IfStatement : Statement
    {
        public Expression Condition { get; set; }

        public AstNode Body { get; set; }
        public Statement Else { get; set; }
    }
}
