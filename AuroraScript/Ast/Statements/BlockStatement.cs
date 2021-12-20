

namespace AuroraScript.Ast.Statements
{
    internal class BlockStatement : Statement
    {
        Scope currentScope;
        public BlockStatement(Scope currentScope)
        {
            this.currentScope = currentScope;
        }


    }
}
