using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    internal class NameExpression :Expression
    {
        /// <summary>
        /// member name
        /// </summary>
        public Token Identifier { get; set; }




    }
}
