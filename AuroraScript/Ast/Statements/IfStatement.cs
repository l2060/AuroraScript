using AuroraScript.Ast.Expressions;

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



        public override String ToString()
        {
            var temp = $"{Symbols.KW_IF.Name}({this.Condition}){this.Body}";
            if(this.Else != null)
            {
                temp += $" {Symbols.KW_ELSE.Name} {this.Else}";
            }
            return temp;
        }

        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.KW_IF.Name);
            writer.Write(" (");
            this.Condition.WriteCode(writer);
            writer.Write(") ");
            this.Body.WriteCode(writer);
            if (this.Else != null)
            {
                writer.WriteLine();
                writer.Write(Symbols.KW_ELSE.Name);
                this.Else.WriteCode(writer);
            }
        }

    }
}
