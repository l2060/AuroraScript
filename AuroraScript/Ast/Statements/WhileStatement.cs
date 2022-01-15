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
            var temp = $"{Symbols.KW_WHILE.Name}({this.Condition}){this.Body}";
            return temp;
        }
    }
}
