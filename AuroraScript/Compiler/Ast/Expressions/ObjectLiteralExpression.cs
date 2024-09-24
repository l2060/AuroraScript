using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class ObjectLiteralExpression : OperatorExpression
    {
        internal ObjectLiteralExpression(Operator @operator) : base(@operator)
        {
   
        }



        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.WriteLine(Operator.ObjectLiteral.Symbol.Name);
            using (writer.IncIndented()) writeParameters(writer, ChildNodes, () =>
            {
                writer.WriteLine(Symbols.PT_COMMA.Name);
            });
            writer.Write(Operator.ObjectLiteral.SecondarySymbols.Name);
        }
    }
}
