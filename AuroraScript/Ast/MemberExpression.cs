using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    /// <summary>
    /// 成员函数表达式
    /// </summary>
    internal class MemberExpression
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
