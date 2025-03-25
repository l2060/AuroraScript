using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public class LambdaExpression : Expression
    {
        public FunctionDeclaration Function;

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            this.Function.GenerateCode(writer);
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitLambdaExpression(this);
        }
    }
}