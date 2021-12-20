using AuroraScript.Ast.Expressions;

namespace AuroraScript.Ast.Statements
{
    internal class IfStatement : Statement
    {
        public IfStatement()
        {
            this.Body = new List<Statement>();
        }
        public Expression Condition { get; set; }
        public List<Statement> Body { get; set; }
        public Statement Else { get; set; }
    }
}
