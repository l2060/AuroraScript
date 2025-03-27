using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;


namespace AuroraScript.Ast.Statements
{
    public class IfStatement : Statement
    {
        internal IfStatement(Expression condition, Statement body, Statement else1)
        {
            Condition = condition;
            Body = body;
            Else = else1;
        }

        public Expression Condition { get; set; }
        public Statement Body { get; set; }
        public Statement Else { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitIfStatement(this);
        }
    }
}