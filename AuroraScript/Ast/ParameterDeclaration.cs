using AuroraScript.Ast.Expressions;


namespace AuroraScript.Ast
{
    /// <summary>
    /// function parameter declaration
    /// </summary>
    public class ParameterDeclaration : AstNode
    {
        internal ParameterDeclaration()
        {

        }
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
