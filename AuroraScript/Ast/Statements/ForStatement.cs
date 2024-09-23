using AuroraScript.Ast.Expressions;
using AuroraScript.Stream;

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


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_FOR.Name);
            writer.Write(" {0}", Symbols.PT_LEFTPARENTHESIS.Name);
            this.Initializer.GenerateCode(writer);
            writer.Write("{0} ", Symbols.PT_SEMICOLON.Name);
            this.Condition.GenerateCode(writer);
            writer.Write("{0} ", Symbols.PT_SEMICOLON.Name);
            this.Incrementor.GenerateCode(writer);
            writer.Write("{0} ", Symbols.PT_RIGHTPARENTHESIS.Name);

            this.Body.GenerateCode(writer);
            if (this.Body is BlockStatement) writer.WriteLine();
        }
    }
}
