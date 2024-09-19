using AuroraScript.Ast.Statements;
using AuroraScript.Ast.Types;
using AuroraScript.Stream;



namespace AuroraScript.Ast.Expressions
{
    public class TypeDeclaration : Expression
    {
        internal TypeDeclaration()
        {

        }

        public Symbols Access { get; set; }
        public Token Identifier { get; set; }
        public ObjectType Typed { get; set; }

        public override void GenerateCode(CodeWriter writer, int depth = 0)
        {
            writer.Write("{0} {1} {2} {3} ", Access.Name, Symbols.KW_TYPE.Name, Identifier.Value, Symbols.OP_ASSIGNMENT.Name);
            Typed.GenerateCode(writer, depth);
        }
    }
}
