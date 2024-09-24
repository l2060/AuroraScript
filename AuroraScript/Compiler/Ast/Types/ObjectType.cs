
using AuroraScript.Compiler;
using AuroraScript.Stream;


namespace AuroraScript.Ast.Types
{
    public class ObjectType : AstNode
    {
        internal ObjectType(Token typeToken)
        {
            ElementType = typeToken;
        }



        /// <summary>
        /// function name
        /// </summary>
        public Token ElementType { get; set; }


        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(ElementType.Value);
        }

    }
}
