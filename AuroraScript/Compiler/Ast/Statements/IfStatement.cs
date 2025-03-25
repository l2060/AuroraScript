using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;
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

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_IF.Name);
            writer.Write(" {0}", Symbols.PT_LEFTPARENTHESIS.Name);
            this.Condition.GenerateCode(writer);
            writer.Write("{0} ", Symbols.PT_RIGHTPARENTHESIS.Name);
            if (this.Else != null)
            {
                writer.WriteLine();
                using (writer.IncIndented(!(this.Body is BlockStatement))) this.Body.GenerateCode(writer);
            }
            else
            {
                this.Body.GenerateCode(writer);
            }
            if (this.Else != null)
            {
                writer.Write(Symbols.KW_ELSE.Name + " ");
                if (!(this.Else is IfStatement)) writer.WriteLine();
                using (writer.IncIndented(!(this.Else is BlockStatement))) this.Else.GenerateCode(writer);
            }
        }
        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitIfStatement(this);
        }
    }
}