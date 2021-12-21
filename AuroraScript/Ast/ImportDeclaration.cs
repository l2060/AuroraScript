using AuroraScript.Ast.Statements;


namespace AuroraScript.Ast
{
    public class ImportDeclaration: Statement
    {
        internal ImportDeclaration()
        {

        }

        public Token Module { get; set; }
        public Token File { get; set; }
    }
}
