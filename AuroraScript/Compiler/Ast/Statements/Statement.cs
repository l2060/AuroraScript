using AuroraScript.Compiler;

namespace AuroraScript.Ast.Statements
{
    public abstract class Statement : AstNode
    {
        internal Statement()
        {
        }

    }

    public class Statements : Statement
    {
        public override void Accept(IAstVisitor visitor)
        {
            foreach (var item in ChildNodes)
            {
                item.Accept(visitor);
            }
        }
    }

}