using AuroraScript.Compiler;
using AuroraScript.Stream;
using AuroraScript.Tokens;


namespace AuroraScript.Ast.Types
{
    public class KeywordTypeNode : TypeNode
    {
        public KeywordTypeNode(Token name)
        {
            this.Name = name;
        }



        public Token Name { get; set; }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Name.Value);
        }

    }
}
