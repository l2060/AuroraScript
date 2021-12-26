
namespace AuroraScript.Ast.Statements
{
    public class BreakStatement : Statement
    {
        internal BreakStatement()
        {

        }
        public override String ToString()
        {
            return $"break;\r\n";
        }
    }
}
