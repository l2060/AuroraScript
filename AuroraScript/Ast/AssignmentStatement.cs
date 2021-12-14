using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    /// <summary>
    /// 赋值
    /// </summary>
    internal class AssignmentStatement : Expression
    {
        public List<Token> Variables { get; set; }

        public List<AstNode> Inits { get; set; }



    }
}
