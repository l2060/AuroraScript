using AuroraScript.Ast.Expressions;

namespace AuroraScript.Ast.Statements
{
    public class ForStatement : Statement
    {
        internal ForStatement()
        {

        }
        public Expression Condition { get; set; }

        public Statement Body { get; set; }


        /// <summary>
        /// for initializer
        /// may be assignment
        /// may be variable declaration
        /// </summary>
        public AstNode Initializer { get; set; }


        /// <summary>
        /// for incrementor
        /// contains multiple sentences 
        /// </summary>
        public Expression Incrementor { get; set; }



        public override String ToString()
        {
            var temp = $"{Symbols.KW_FOR.Name}({this.Initializer}{this.Condition}{Symbols.PT_SEMICOLON.Name}{this.Incrementor}){this.Body}";
            return temp;
        }


        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_FOR.Name);
            writer.Write("(");
            this.Initializer.WriteCode(writer);
            writer.Write("; ");
            this.Condition.WriteCode(writer);
            writer.Write("; ");
            this.Incrementor.WriteCode(writer);
            writer.Write(")");

            this.Body.WriteCode(writer);
        }
    }
}
