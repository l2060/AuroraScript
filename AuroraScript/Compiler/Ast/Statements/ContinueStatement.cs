using AuroraScript.Compiler;


namespace AuroraScript.Ast.Statements
{
    public class ContinueStatement : Statement
    {
        internal ContinueStatement()
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitContinueExpression(this);
        }
    }
}