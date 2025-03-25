using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class CastTypeExpression : PrefixUnaryExpression
    {
        internal CastTypeExpression(Operator @operator) : base(@operator)
        {
        }

        public Expression Typed { get; set; }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Operator.CastType.Symbol.Name);
            Typed.GenerateCode(writer, depth);
            writer.Write(Operator.CastType.SecondarySymbols.Name);
            Right.GenerateCode(writer, depth);
        }
        public override void Accept(IAstVisitor visitor)
        {

        }
    }
}