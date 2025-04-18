using AuroraScript.Ast.Statements;

namespace AuroraScript.Compiler.Ast.Statements
{

    public class YieldStatement : Statement
    {
        internal YieldStatement()
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitYieldExpression(this);
        }
    }
}