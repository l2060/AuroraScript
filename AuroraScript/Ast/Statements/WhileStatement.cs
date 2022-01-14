using AuroraScript.Ast.Expressions;


namespace AuroraScript.Ast.Statements
{
    public class WhileStatement : Statement
    {
        internal WhileStatement()
        {

        }

        public Expression Condition { get; set; }

        public Statement Body { get; set; }


        public override String ToString()
        {
            var temp = $"while({this.Condition}){this.Body}";
            return temp;
        }
    }
}
