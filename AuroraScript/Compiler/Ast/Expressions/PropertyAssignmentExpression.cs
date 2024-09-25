using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    internal class PropertyAssignmentExpression : BinaryExpression
    {
        internal PropertyAssignmentExpression(Operator @operator) : base(@operator)
        {
        }

        public Token Key { get; set; }
        public AstNode Value { get; set; }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write($"{Key?.Value}{this.Operator.Symbol.Name} ");
            Value?.GenerateCode(writer, depth);
        }
    }
}