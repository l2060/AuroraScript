
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class NameExpression :Expression
    {
        /// <summary>
        /// member name
        /// </summary>
        public Token Identifier { get; set; }


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.Write(this.Identifier.Value);
        }
    }
}
