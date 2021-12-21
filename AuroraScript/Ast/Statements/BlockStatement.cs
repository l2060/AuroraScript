

namespace AuroraScript.Ast.Statements
{
    public class BlockStatement : Statement
    {
        public Scope currentScope { get; private set; }
        internal BlockStatement(Scope currentScope)
        {
            this.currentScope = currentScope;
        }

        public virtual IEnumerable<AstNode> ChildNodes
        {
            get
            {
                return childrens;
            }
        }
    }
}
