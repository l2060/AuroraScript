using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Common;

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
        public ObjectType Typed { get; set; }

        /// <summary>
        /// Function Access
        /// </summary>
        public Symbols Access { get; set; }

        public Boolean IsConst { get; set; }



        public override String ToString()
        {
            var key = IsConst ? "const" : "var";
            return $"{key} {String.Join(',', Variables.Select(e => e.Value))}:{Typed} = {Initializer};";
        }



    }
}
