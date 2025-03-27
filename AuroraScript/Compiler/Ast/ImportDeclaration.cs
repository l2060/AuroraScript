using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;


namespace AuroraScript.Ast
{
    public class ImportDeclaration : Statement
    {
        internal ImportDeclaration()
        {
        }

        public Token Module { get; set; }
        public Token File { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
        }
    }
}