
namespace AuroraScript.Ast.Expressions
{
    internal class Expression : AstNode
    {
        /// <summary>
        /// function result types
        /// </summary>
        public List<Token> Types { get; set; }

        public new Expression this[Int32 index]
        {
            get
            {
                return (Expression)this.childrens[index];
            }
        }


        public Expression Pop()
        {
            var node = this.childrens[0];
            node.Remove();
            return (Expression)node;
        }


    }
}
