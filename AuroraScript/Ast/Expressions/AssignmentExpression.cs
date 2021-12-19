using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 赋值
    /// </summary>
    internal class AssignmentExpression : OperatorExpression
    {
        public AssignmentExpression(Operator @operator) : base(@operator)
        {
        }

        public List<Token> Variables { get; set; }

        public List<AstNode> Inits { get; set; }



    }
}
