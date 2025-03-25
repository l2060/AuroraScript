using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;


namespace AuroraScript.Compiler
{
    public interface IAstVisitor
    {
        void VisitProgram(ModuleDeclaration node);
        void VisitFunction(FunctionDeclaration node);

        void VisitLambdaExpression(LambdaExpression node);
        void VisitBlock(BlockStatement node);

        void VisitName(NameExpression node);
        void VisitVarDeclaration(VariableDeclaration node);
        void VisitIfStatement(IfStatement node);
        void VisitWhileStatement(WhileStatement node);
        void VisitForStatement(ForStatement node);


        void VisitReturnStatement(ReturnStatement node);
        //void VisitExpressionStatement(ExpressionStatementNode node);
        //void VisitVariableExpression(NameExpression node);
        void VisitAssignmentExpression(AssignmentExpression node);
        //void VisitLogicalExpression(LogicalExpressionNode node);



        void VisitBinaryExpression(BinaryExpression node);
        void VisitUnaryExpression(UnaryExpression node);
        void VisitCallExpression(FunctionCallExpression node);
        void VisitGetPropertyExpression(MemberAccessExpression node);
        void VisitSetPropertyExpression(PropertyAssignmentExpression node);
        //void VisitGetElementExpression(GetElementExpressionNode node);
        //void VisitSetElementExpression(SetElementExpressionNode node);
        void VisitLiteralExpression(LiteralExpression node);
        void VisitGroupingExpression(GroupExpression node);
        void VisitArrayExpression(ArrayLiteralExpression node);

        void VisitArrayAccessExpression(ArrayAccessExpression node);

        void VisitMapExpression(ObjectLiteralExpression node);

        void VisitDeconstructionExpression(DeconstructionExpression node);
        void VisitBreakExpression(BreakStatement node);
        void VisitContinueExpression(ContinueStatement node);


    }
}
