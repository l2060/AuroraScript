using AuroraScript.Ast.Statements;

namespace AuroraScript.Ast.Expressions
{
    internal class ConditionExpression : Expression
    {
        public Expression Condition { get; set; }

        public BlockStatement Body { get; set; }


    }
}
