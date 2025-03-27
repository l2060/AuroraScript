using AuroraScript.Compiler;


namespace AuroraScript.Ast.Statements
{
    public class BlockStatement : Statement
    {
        /// <summary>
        /// statement scope
        /// </summary>
        public Scope Scope { get; private set; }
        public readonly List<FunctionDeclaration> Functions = new List<FunctionDeclaration>();
        internal BlockStatement(Scope currentScope)
        {
            this.Scope = currentScope;
        }

        public virtual new List<AstNode> ChildNodes
        {
            get
            {
                return childrens;
            }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitBlock(this);
        }
    }
}