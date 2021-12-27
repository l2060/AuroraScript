
namespace AuroraScript.Ast.Statements
{
    public class Statement : AstNode
    {
        internal Statement()
        {

        }

        public override String ToString()
        {
            return this.childrens.Count > 0? $"{this.childrens[0]};\r\n" : "**-**";
        }
    }
}
