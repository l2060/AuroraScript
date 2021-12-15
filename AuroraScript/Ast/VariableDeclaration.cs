using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    /// <summary>
    /// 变量声明
    /// </summary>
    internal class VariableDeclaration : Expression
    {

        public List<Token> Variables { get; set; }


        public List<AstNode> Inits { get; set; }



    }
}
