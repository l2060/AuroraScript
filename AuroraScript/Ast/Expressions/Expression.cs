using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    internal class Expression : AstNode
    {
        /// <summary>
        /// function result types
        /// </summary>
        public List<Token> Types { get; set; }


    }
}
