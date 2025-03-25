using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class MapKeyValueExpression : BinaryExpression
    {
        internal MapKeyValueExpression(Operator @operator) : base(@operator)
        {
        }

        public Token Key { get; set; }
        public AstNode Value { get; set; }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write($"{Key?.Value}{this.Operator.Symbol.Name} ");
            Value?.GenerateCode(writer, depth);
        }

        public override void Accept(IAstVisitor visitor)
        {
            //visitor.VisitSetPropertyExpression(this);
        }
    }
}