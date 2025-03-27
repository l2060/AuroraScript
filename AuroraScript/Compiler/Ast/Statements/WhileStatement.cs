using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;


namespace AuroraScript.Ast.Statements
{
    public class WhileStatement : Statement
    {
        internal WhileStatement(Expression condition, Statement body)
        {
            Condition = condition;
            Body = body;
        }

        public Expression Condition { get; set; }

        public Statement Body { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitWhileStatement(this);
        }

        public override string ToString()
        {
            return $"whele ({this.Condition}) {this.Body}";
        }


    }
}