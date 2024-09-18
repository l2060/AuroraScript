

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

        public new virtual List<AstNode> ChildNodes
        {
            get
            {
                return childrens;
            }
        }

        public override String ToString()
        {
            var temp = $"{Symbols.PT_LEFTBRACE.Name}";
            foreach (var item in ChildNodes)
            {
                temp += $"\r\n{item}";
            }
            temp += $"\r\n{Symbols.PT_RIGHTBRACE.Name}";
            return temp;
        }

        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.WriteLine(Symbols.PT_LEFTBRACE.Name);
            this.writeParameters(writer, ChildNodes, "");
            writer.WriteLine(Symbols.PT_RIGHTBRACE.Name);
        }




    }
}
