
namespace AuroraScript.Ast.Statements
{
    public class ContinueStatement : Statement
    {
        internal ContinueStatement()
        {

        }
        public override String ToString()
        {
            return $"continue;\r\n";
        }
    }
}
