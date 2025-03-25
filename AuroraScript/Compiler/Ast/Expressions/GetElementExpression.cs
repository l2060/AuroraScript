using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class GetElementExpression : OperatorExpression
    {
        internal GetElementExpression(Operator @operator) : base(@operator)
        {
        }

        public Expression Index { get; set; }

        public Expression Object
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }


        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitGetElementExpression(this);
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            Object.GenerateCode(writer);
            writer.Write(Operator.ArrayLiteral.Symbol.Name);
            Index.GenerateCode(writer);
            writer.Write(Operator.ArrayLiteral.SecondarySymbols.Name);
        }
    }
}