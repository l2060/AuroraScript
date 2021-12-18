using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    /// <summary>
    /// 变量声明
    /// </summary>
    internal class VariableDeclaration : Statement
    {
        public VariableDeclaration()
        {
            this.Variables = new List<Token>();
        }
        public List<Token> Variables { get; set; }


        public Expression Initializer { get; set; }



    }
}
