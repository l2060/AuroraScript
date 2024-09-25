using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class NOPExpression : Expression
    {
        public Token Identifier { get; set; }

        public List<AstNode> Arguments { get; set; }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
        }
    }
}