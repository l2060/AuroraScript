using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    public class LambdaExpression : Expression
    {
        public FunctionDeclaration Function;
        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptLambdaExpression(this);
        }
    }
}