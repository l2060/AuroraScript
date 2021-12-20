using AuroraScript.Ast.Statements;


namespace AuroraScript.Ast
{
    internal class ImportDeclaration: Statement
    {
        public Token File { get; set; }
    }
}
