using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    internal class LambdaExpression : Expression
    {
        public FunctionDeclaration Function;
        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptLambdaExpression(this);
        }
    }
}