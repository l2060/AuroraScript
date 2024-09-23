using AuroraScript.Ast.Expressions;
using AuroraScript.Stream;


namespace AuroraScript.Ast.Statements
{
    public class WhileStatement : Statement
    {
        internal WhileStatement()
        {

        }

        public Expression Condition { get; set; }

        public Statement Body { get; set; }


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_WHILE.Name);
            writer.Write(" {0}", Symbols.PT_LEFTPARENTHESIS.Name);
            this.Condition.GenerateCode(writer);
            writer.Write("{0} ", Symbols.PT_RIGHTPARENTHESIS.Name);
            this.Body.GenerateCode(writer);
            if(this.Body is BlockStatement) writer.WriteLine();
        }



    }
}
