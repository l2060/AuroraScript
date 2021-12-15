using AuroraScript.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Statements
{
    internal class ForStatement : ConditionExpression
    {
        /// <summary>
        /// for initializer
        /// may be assignment
        /// may be variable declaration
        /// </summary>
        public Expression Initializer { get; set; }


        /// <summary>
        /// for incrementor
        /// contains multiple sentences 
        /// </summary>
        public BlockStatement Incrementor { get; set; }




    }
}
