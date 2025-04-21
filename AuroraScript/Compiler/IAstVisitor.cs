using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;
using AuroraScript.Compiler.Ast.Statements;


namespace AuroraScript.Compiler
{
    public abstract class IAstVisitor
    {



        public void AcceptImportDeclaration(ImportDeclaration node)
        {
            BeforeVisitNode(node);
            VisitImportDeclaration(node);
            AfterVisitNode(node);
        }


        public void AcceptModule(ModuleDeclaration node)
        {
            BeforeVisitNode(node);
            VisitModule(node);
            AfterVisitNode(node);
        }

        public void AcceptFunction(FunctionDeclaration node)
        {
            BeforeVisitNode(node);
            VisitFunction(node);
            AfterVisitNode(node);
        }


        public void AcceptLambdaExpression(LambdaExpression node)
        {
            BeforeVisitNode(node);
            VisitLambdaExpression(node);
            AfterVisitNode(node);
        }


        public void AcceptBlock(BlockStatement node)
        {
            BeforeVisitNode(node);
            VisitBlock(node);
            AfterVisitNode(node);
        }


        public void AcceptName(NameExpression node)
        {
            BeforeVisitNode(node);
            VisitName(node);
            AfterVisitNode(node);
        }


        public void AcceptVarDeclaration(VariableDeclaration node)
        {
            BeforeVisitNode(node);
            VisitVarDeclaration(node);
            AfterVisitNode(node);
        }

        public void AcceptIfStatement(IfStatement node)
        {
            BeforeVisitNode(node);
            VisitIfStatement(node);
            AfterVisitNode(node);
        }

        public void AcceptWhileStatement(WhileStatement node)
        {
            BeforeVisitNode(node);
            VisitWhileStatement(node);
            AfterVisitNode(node);
        }

        public void AcceptForStatement(ForStatement node)
        {
            BeforeVisitNode(node);
            VisitForStatement(node);
            AfterVisitNode(node);
        }

        public void AcceptForInStatement(ForInStatement node)
        {
            BeforeVisitNode(node);
            VisitForInStatement(node);
            AfterVisitNode(node);
        }

        public void AcceptInExpression(InExpression node)
        {
            BeforeVisitNode(node);
            VisitInExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptReturnStatement(ReturnStatement node)
        {
            BeforeVisitNode(node);
            VisitReturnStatement(node);
            AfterVisitNode(node);
        }

        public void AcceptDeleteStatement(DeleteStatement node)
        {
            BeforeVisitNode(node);
            VisitDeleteStatement(node);
            AfterVisitNode(node);
        }

        public void AcceptAssignmentExpression(AssignmentExpression node)
        {
            BeforeVisitNode(node);
            VisitAssignmentExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptCompoundExpression(CompoundExpression node)
        {
            BeforeVisitNode(node);
            VisitCompoundExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptBinaryExpression(BinaryExpression node)
        {
            BeforeVisitNode(node);
            VisitBinaryExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptUnaryExpression(UnaryExpression node)
        {
            BeforeVisitNode(node);
            VisitUnaryExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptCallExpression(FunctionCallExpression node)
        {
            BeforeVisitNode(node);
            VisitCallExpression(node);
            AfterVisitNode(node);
        }


        public void AcceptLiteralExpression(LiteralExpression node)
        {
            BeforeVisitNode(node);
            VisitLiteralExpression(node);
            AfterVisitNode(node);
        }
        public void AcceptGroupingExpression(GroupExpression node)
        {
            BeforeVisitNode(node);
            VisitGroupingExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptArrayExpression(ArrayLiteralExpression node)
        {
            BeforeVisitNode(node);
            VisitArrayExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptGetElementExpression(GetElementExpression node)
        {
            BeforeVisitNode(node);
            VisitGetElementExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptSetElementExpression(SetElementExpression node)
        {
            BeforeVisitNode(node);
            VisitSetElementExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptGetPropertyExpression(GetPropertyExpression node)
        {
            BeforeVisitNode(node);
            VisitGetPropertyExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptSetPropertyExpression(SetPropertyExpression node)
        {
            BeforeVisitNode(node);
            VisitSetPropertyExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptMapExpression(MapExpression node)
        {
            BeforeVisitNode(node);
            VisitMapExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptDeconstructionExpression(DeconstructionExpression node)
        {
            BeforeVisitNode(node);
            VisitDeconstructionExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptBreakExpression(BreakStatement node)
        {
            BeforeVisitNode(node);
            VisitBreakExpression(node);
            AfterVisitNode(node);
        }
        
        public void AcceptContinueExpression(ContinueStatement node)
        {
            BeforeVisitNode(node);
            VisitContinueExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptYieldExpression(YieldStatement node)
        {
            BeforeVisitNode(node);
            VisitYieldExpression(node);
            AfterVisitNode(node);
        }

        public void AcceptParameterDeclaration(ParameterDeclaration node)
        {
            BeforeVisitNode(node);
            VisitParameterDeclaration(node);
            AfterVisitNode(node);
        }







        protected virtual void BeforeVisitNode(AstNode node)
        {

        }

        protected virtual void AfterVisitNode(AstNode node)
        {

        }


        protected virtual void VisitImportDeclaration(ImportDeclaration node)
        {

        }

        protected virtual void VisitModule(ModuleDeclaration node)
        {
            foreach (var module in node.Imports)
            {
                module.Accept(this);
            }
            VisitBlock(node);
            foreach (var function in node.Functions)
            {
                function.Accept(this);
            }
        }

        protected virtual void VisitFunction(FunctionDeclaration node)
        {
            foreach (var statement in node.Parameters)
            {
                statement.Accept(this);
            }
            node.Body?.Accept(this);
        }


        protected virtual void VisitLambdaExpression(LambdaExpression node)
        {
            node.Function.Accept(this);
        }


        protected virtual void VisitBlock(BlockStatement node)
        {
            foreach (var statement in node.ChildNodes)
            {
                statement.Accept(this);
            }
            foreach (var function in node.Functions)
            {
                function.Accept(this);
            }
        }


        protected virtual void VisitName(NameExpression node)
        {

        }


        protected virtual void VisitVarDeclaration(VariableDeclaration node)
        {
            node.Initializer?.Accept(this);
        }

        protected virtual void VisitIfStatement(IfStatement node)
        {
            node.Condition.Accept(this);
            node.Body?.Accept(this);
            node.Else?.Accept(this);
        }
        protected virtual void VisitWhileStatement(WhileStatement node)
        {
            node.Condition.Accept(this);
            node.Body.Accept(this);
        }
        protected virtual void VisitForStatement(ForStatement node)
        {
            node.Initializer?.Accept(this);
            node.Condition?.Accept(this);
            node.Body.Accept(this);
            node.Incrementor?.Accept(this);
        }



        protected virtual void VisitForInStatement(ForInStatement node)
        {

        }

        protected virtual void VisitInExpression(InExpression node)
        {

        }



        protected virtual void VisitReturnStatement(ReturnStatement node)
        {
            node.Expression?.Accept(this);
        }


        protected virtual void VisitDeleteStatement(DeleteStatement node)
        {
            node.Expression?.Accept(this);
        }


        protected virtual void VisitAssignmentExpression(AssignmentExpression node)
        {
            node.Right.Accept(this);
            node.Left.ToString();
        }


        protected virtual void VisitCompoundExpression(CompoundExpression node)
        {

        }

        protected virtual void VisitBinaryExpression(BinaryExpression node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        protected virtual void VisitUnaryExpression(UnaryExpression node)
        {
            var exp = node.ChildNodes[0];
            exp.Accept(this);
        }

        protected virtual void VisitCallExpression(FunctionCallExpression node)
        {
            foreach (var arg in node.Arguments)
            {
                arg.Accept(this);
            }
            node.Target.Accept(this);
        }


        protected virtual void VisitLiteralExpression(LiteralExpression node)
        {

        }
        protected virtual void VisitGroupingExpression(GroupExpression node)
        {
            foreach (var item in node.ChildNodes)
            {
                item.Accept(this);
            }
        }

        protected virtual void VisitArrayExpression(ArrayLiteralExpression node)
        {
            foreach (var item in node.ChildNodes)
            {
                item.Accept(this);
            }
        }

        protected virtual void VisitGetElementExpression(GetElementExpression node)
        {
            node.Object.Accept(this);
            if (node.Index is NameExpression name)
            {
                // get property
                VisitName(name);
            }
            else if (node.Index is LiteralExpression literal)
            {
                VisitLiteralExpression(literal);
            }
            else
            {
                node.Index.Accept(this);
            }
        }
        protected virtual void VisitSetElementExpression(SetElementExpression node)
        {
            node.Value.Accept(this);
            if (node.Index is NameExpression name)
            {
                VisitName(name);
                node.Object.Accept(this);
            }
            else if (node.Index is LiteralExpression literal)
            {
                VisitLiteralExpression(literal);
                node.Object.Accept(this);
            }
            else
            {
                node.Index.Accept(this);
            }
        }
        protected virtual void VisitGetPropertyExpression(GetPropertyExpression node)
        {
            node.Object.Accept(this);
            node.Property.Accept(this);
        }

        protected virtual void VisitSetPropertyExpression(SetPropertyExpression node)
        {
            node.Object.Accept(this);
            // Compile the value expression
            node.Value.Accept(this);
            node.Property.Accept(this);
        }


        protected virtual void VisitMapExpression(MapExpression node)
        {
            foreach (var entry in node.ChildNodes)
            {
                if (entry is MapKeyValueExpression property)
                {
                    property.Value.Accept(this);
                }
            }
        }

        protected virtual void VisitDeconstructionExpression(DeconstructionExpression node)
        {

        }
        protected virtual void VisitBreakExpression(BreakStatement node)
        {

        }
        protected virtual void VisitContinueExpression(ContinueStatement node)
        {

        }

        protected virtual void VisitYieldExpression(YieldStatement node)
        {

        }

        protected virtual void VisitParameterDeclaration(ParameterDeclaration node)
        {
            node.Initializer?.Accept(this);
        }


    }
}
