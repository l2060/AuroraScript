using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    public abstract class Expression : AstNode
    {
        internal Expression()
        {
        }

        public new Expression this[Int32 index]
        {
            get
            {
                return (Expression)this.childrens[index];
            }
        }

        internal Expression Pop()
        {
            var node = this.childrens[0];
            node.Remove();
            return (Expression)node;
        }
    }


    internal class ExpressionStack : Expression
    {
        public override void Accept(IAstVisitor visitor)
        {

        }
    }

}