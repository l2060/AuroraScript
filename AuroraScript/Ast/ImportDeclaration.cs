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
                return $"{Symbols.KW_IMPORT.Name} {this.Module.Value} {Symbols.KW_FROM.Name} {this.File.Value};";
            }
            else
            {
                return $"{Symbols.KW_IMPORT.Name} {this.File.Value};";
            }
        }



        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            if (this.Module != null)
            {
                writer.WriteLine($"{Symbols.KW_IMPORT.Name} {this.Module.Value} {Symbols.KW_FROM.Name} {this.File.Value};");
            }
            else
            {
                writer.WriteLine($"{Symbols.KW_IMPORT.Name} {this.File.Value};");
            }
        }


    }
}
