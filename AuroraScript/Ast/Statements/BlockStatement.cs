

namespace AuroraScript.Ast.Statements
{
    public class BlockStatement : Statement
    {
        /// <summary>
        /// statement scope
        /// </summary>
        public Scope Scope { get; private set; }
        internal BlockStatement(Scope currentScope)
        {
            this.Scope = currentScope;
        }

        public new virtual IEnumerable<AstNode> ChildNodes
        {
            get
            {
                return childrens;
            }
        }

        public override String ToString()
        {
            var temp = "{";
            foreach (var item in ChildNodes)
            {
                temp += $"\r\n{item}";
            }
            temp += "\r\n}\r\n";
            return temp;
        }

    }
}
