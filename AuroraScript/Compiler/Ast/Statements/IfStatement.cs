using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;


namespace AuroraScript.Ast.Statements
{
    internal class IfStatement : Statement
    {
        internal IfStatement(Expression condition, Statement body, Statement else1)
        {
            Condition = condition;
            Body = body;
            Else = else1;
            condition.Parent = this;
        }

        public Expression Condition { get; set; }
        public Statement Body { get; set; }
        public Statement Else { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptIfStatement(this);
        }
    }
}