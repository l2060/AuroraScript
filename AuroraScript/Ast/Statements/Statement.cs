
namespace AuroraScript.Ast.Statements
{
    public class Statement : AstNode
    {
        internal Statement()
        {

        }

        public override String ToString()
        {
            return $"{this.childrens[0]};";
        }
    }
}
