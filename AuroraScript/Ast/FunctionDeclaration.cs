using AuroraScript.Ast.Statements;


namespace AuroraScript.Ast
{
    /// <summary>
    /// 函数定义
    /// </summary>
    internal class FunctionDeclaration : Statement
    {
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
        public Token Type { get; set; }

    }
}
