using AuroraScript.Ast.Expressions;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Statements
{
    public class IfStatement : Statement
    {
        internal IfStatement()
        {
        }
        public Expression Condition { get; set; }
        public Statement Body { get; set; }
        public Statement Else { get; set; }


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_IF.Name);
            writer.Write(" {0}", Symbols.PT_LEFTPARENTHESIS.Name);
            this.Condition.GenerateCode(writer);
            writer.Write("{0} ", Symbols.PT_RIGHTPARENTHESIS.Name);
            if (this.Else != null)
            {
                writer.WriteLine();
                using (writer.IncIndented()) this.Body.GenerateCode(writer);
            }
            else
            {
                this.Body.GenerateCode(writer);
            }
            if (this.Else != null)
            {
                writer.WriteLine();
                writer.Write(Symbols.KW_ELSE.Name + " ");
                writer.WriteLine();
                using (writer.IncIndented()) this.Else.GenerateCode(writer);
            }
        }

    }
}
