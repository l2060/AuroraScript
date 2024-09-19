using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Types;
using AuroraScript.Stream;


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


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Variable.Value);
            if (Typed != null)
            {
                writer.Write($"{Symbols.PT_COLON.Name} ");
                Typed.GenerateCode(writer, depth);
            }
            if (DefaultValue != null) {
                writer.Write($" {Symbols.OP_ASSIGNMENT.Name} {DefaultValue}");
            }
        }



    }
}
