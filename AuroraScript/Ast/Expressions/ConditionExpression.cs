using AuroraScript.Ast.Statements;

namespace AuroraScript.Ast.Expressions
{
    public class ConditionExpression : Expression
    {
        internal ConditionExpression()
        {

        }
        public Expression Condition { get; set; }

        public BlockStatement Body { get; set; }


    }
}
