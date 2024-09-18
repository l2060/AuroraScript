
namespace AuroraScript.Ast.Expressions
{
    public class NameExpression :Expression
    {
        /// <summary>
        /// member name
        /// </summary>
        public Token Identifier { get; set; }



        public override String ToString()
        {
            return $"{this.Identifier.Value}";
        }

        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.Write(this.Identifier.Value);
        }
    }
}
