using AuroraScript.Ast.Expressions;


namespace AuroraScript.Ast
{
    /// <summary>
    /// function parameter declaration
    /// </summary>
    internal class ParameterDeclaration : AstNode
    {
        /// <summary>
        /// parameter Modifier  ....
        /// </summary>
        public Token Modifier { get; set; }

        /// <summary>
        /// Parameter
        /// </summary>
        public Token Variable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Expression DefaultValue { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Token Typed { get; set; }



    }
}
