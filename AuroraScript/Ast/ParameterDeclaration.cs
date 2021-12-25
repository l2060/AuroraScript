using AuroraScript.Ast.Expressions;
using AuroraScript.Common;

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
        public ObjectType Typed { get; set; }



        public override String ToString()
        {
            var temp = $"{Variable.Value} function {Typed}";

            if(DefaultValue != null)
            {
                temp += $" = {DefaultValue}";
            }
            return temp;
        }

    }
}
