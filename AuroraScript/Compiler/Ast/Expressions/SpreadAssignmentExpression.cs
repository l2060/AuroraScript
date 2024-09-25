using AuroraScript.Compiler;

namespace AuroraScript.Ast.Expressions
{
    internal class SpreadAssignmentExpression : PrefixUnaryExpression
    {
        internal SpreadAssignmentExpression() : base(Operator.PreSpread)
        {
        }
    }
}