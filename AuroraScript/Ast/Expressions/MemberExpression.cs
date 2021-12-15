using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 成员函数表达式
    /// </summary>
    internal class MemberExpression : Expression
    {
        /// <summary>
        /// member name
        /// </summary>
        public Token Identifier { get; set; }

        /// <summary>
        /// super object name
        /// </summary>
        public AstNode Base { get; set; }
    }

}
