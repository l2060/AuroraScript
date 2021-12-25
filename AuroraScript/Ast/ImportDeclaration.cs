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


        public override String ToString()
        {
            if (this.Module != null)
            {
                return $"import {this.Module.Value} from {this.File.Value};";
            }
            else
            {
                return $"import {this.File.Value};";
            }
        }
    }
}
