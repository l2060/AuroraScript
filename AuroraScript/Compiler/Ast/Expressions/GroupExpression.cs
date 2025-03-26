using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class GroupExpression : OperatorExpression
    {
        internal GroupExpression(Operator @operator) : base(@operator)
        {
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            writer.Write(Symbols.PT_LEFTPARENTHESIS.Name);
            writeParameters(writer, ChildNodes, ", ");
            writer.Write(Symbols.PT_RIGHTPARENTHESIS.Name);
        }
        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitGroupingExpression(this);
        }
    }
}