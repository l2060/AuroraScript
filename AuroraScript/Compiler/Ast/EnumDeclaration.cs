using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast
{
    public class EnumElement
    {
        public Token Name;
        public Int32 Value;
    }

    public class EnumDeclaration : Statement
    {
        internal EnumDeclaration()
        {
            this.Access = Symbols.KW_INTERNAL;
        }

        /// <summary>
        /// Function Access
        /// </summary>
        public Symbols Access { get; set; }

        public Token Identifier { get; set; }
        public List<EnumElement> Elements { get; set; }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.WriteLine($"{this.Access.Name} {Symbols.KW_ENUM.Name} {Identifier.Value} {Symbols.PT_LEFTBRACE.Name}");
            using (writer.IncIndented())
            {
                foreach (EnumElement element in Elements)
                {
                    writer.WriteLine($"{element.Name.Value} = {element.Value},");
                }
            }
            writer.WriteLine(Symbols.PT_RIGHTBRACE.Name);
        }

        public override void Accept(IAstVisitor visitor)
        {
        }
    }
}