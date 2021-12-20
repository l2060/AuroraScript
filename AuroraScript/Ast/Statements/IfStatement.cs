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
    }
}
