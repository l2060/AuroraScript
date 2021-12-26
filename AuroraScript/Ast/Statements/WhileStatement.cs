using AuroraScript.Ast.Expressions;


namespace AuroraScript.Ast.Statements
{
    public class WhileStatement : Statement
    {
        internal WhileStatement()
        {

        }


        public Expression Condition { get; set; }

        public BlockStatement Body { get; set; }
    }
}
