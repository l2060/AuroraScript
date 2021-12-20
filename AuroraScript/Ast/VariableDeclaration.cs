using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;


namespace AuroraScript.Ast
{
    /// <summary>
    /// variable declaration
    /// </summary>
    public class VariableDeclaration : Statement
    {
        internal VariableDeclaration()
        {
            this.Variables = new List<Token>();
        }

        /// <summary>
        /// variable names
        /// </summary>
        public List<Token> Variables { get; set; }


        /// <summary>
        /// var initialize statement
        /// </summary>
        public Expression Initializer { get; set; }


        /// <summary>
        /// get / set variable typed
        /// </summary>
        public Token Typed { get; set; }


    }
}
