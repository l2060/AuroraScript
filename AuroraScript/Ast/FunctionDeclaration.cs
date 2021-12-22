using AuroraScript.Ast.Statements;
using AuroraScript.Common;

namespace AuroraScript.Ast
{
    /// <summary>
    /// 函数定义
    /// </summary>
    public class FunctionDeclaration : Statement
    {
        internal FunctionDeclaration()
        {

        }
        /// <summary>
        /// Function Access
        /// </summary>
        public Symbols Access { get; set; }
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
        public Statement Body { get; set; }

        /// <summary>
        /// function result types
        /// </summary>
        public List<ObjectTyped> Typeds { get; set; }

    }
}
