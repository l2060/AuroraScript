using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    /// <summary>
    /// 函数定义
    /// </summary>
    internal class FunctionDeclaration
    {

        /// <summary>
        /// Export ....
        /// </summary>
        public List<Token> Modifiers { get; set; }

        /// <summary>
        /// parameters
        /// </summary>
        public List<ParameterDeclaration> Parameters { get; set; }

        /// <summary>
        /// function name
        /// </summary>
        public Token Identifier { get; set; }

        /// <summary>
        /// function code
        /// </summary>
        public AstNode Body { get; set; }

        /// <summary>
        /// function result types
        /// </summary>
        public List<Token> Types { get; set; }

    }
}
