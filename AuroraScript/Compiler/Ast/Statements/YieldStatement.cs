using AuroraScript.Ast.Statements;

namespace AuroraScript.Compiler.Ast.Statements
{

    internal class YieldStatement : Statement
    {
        internal YieldStatement()
        {
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptYieldExpression(this);
        }
    }
}