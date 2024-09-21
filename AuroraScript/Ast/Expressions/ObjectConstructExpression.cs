using AuroraScript.Ast.Statements;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class ObjectConstructExpression : OperatorExpression
    {
        internal ObjectConstructExpression(Operator @operator) : base(@operator)
        {
            this.Elements = new List<Expression>();
        }

        public List<Expression> Elements { get; set; }


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            var els = Elements.Select(el => el.ToString()).ToArray();
            writer.WriteLine(Operator.Constructor.Symbol.Name);
            using (writer.IncIndented()) writeParameters(writer, Elements, () =>
            {
                writer.WriteLine(Symbols.PT_COMMA.Name);
            });
            writer.Write(Operator.Constructor.SecondarySymbols.Name);
        }
    }
}
