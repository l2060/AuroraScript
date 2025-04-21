using AuroraScript.Compiler;

namespace AuroraScript.Ast.Statements
{
    public class BreakStatement : Statement
    {
        internal BreakStatement()
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptBreakExpression(this);
        }
    }
}