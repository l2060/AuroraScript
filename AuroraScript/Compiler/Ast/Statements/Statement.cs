using AuroraScript.Compiler;

namespace AuroraScript.Ast.Statements
{
    internal abstract class Statement : AstNode
    {
        internal Statement()
        {
        }

    }

    internal class Statements : Statement
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