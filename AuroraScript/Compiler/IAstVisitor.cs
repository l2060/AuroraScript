using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;


namespace AuroraScript.Compiler
{
    public abstract class IAstVisitor
    {

        public virtual void VisitImportDeclaration(ImportDeclaration node)
        {

        }


        public virtual void VisitModule(ModuleDeclaration node)
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

        public virtual void VisitFunction(FunctionDeclaration node)
        {
            foreach (var statement in node.Parameters)
            {
                statement.Accept(this);
            }
            node.Body?.Accept(this);
        }


        public virtual void VisitLambdaExpression(LambdaExpression node)
        {
            node.Function.Accept(this);
        }


        public virtual void VisitBlock(BlockStatement node)
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


        public virtual void VisitName(NameExpression node)
        {

        }


        public virtual void VisitVarDeclaration(VariableDeclaration node)
        {
            node.Initializer?.Accept(this);
        }

        public virtual void VisitIfStatement(IfStatement node)
        {
            node.Condition.Accept(this);
            node.Body?.Accept(this);
            node.Else?.Accept(this);
        }
        public virtual void VisitWhileStatement(WhileStatement node)
        {
            node.Condition.Accept(this);
            node.Body.Accept(this);
        }
        public virtual void VisitForStatement(ForStatement node)
        {
            node.Initializer?.Accept(this);
            node.Condition?.Accept(this);
            node.Body.Accept(this);
            node.Incrementor?.Accept(this);
        }


        public virtual void VisitReturnStatement(ReturnStatement node)
        {
            node.Expression?.Accept(this);
        }

        public virtual void VisitAssignmentExpression(AssignmentExpression node)
        {
            node.Right.Accept(this);
            node.Left.ToString();
        }


        public virtual void VisitCompoundExpression(CompoundExpression node)
        {

        }

        public virtual void VisitBinaryExpression(BinaryExpression node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        public virtual void VisitUnaryExpression(UnaryExpression node)
        {
            var exp = node.ChildNodes[0];
            exp.Accept(this);
        }

        public virtual void VisitCallExpression(FunctionCallExpression node)
        {
            foreach (var arg in node.Arguments)
            {
                arg.Accept(this);
            }
            node.Target.Accept(this);
        }


        public virtual void VisitLiteralExpression(LiteralExpression node)
        {

        }
        public virtual void VisitGroupingExpression(GroupExpression node)
        {
            foreach (var item in node.ChildNodes)
            {
                item.Accept(this);
            }
        }

        public virtual void VisitArrayExpression(ArrayLiteralExpression node)
        {
            foreach (var item in node.ChildNodes)
            {
                item.Accept(this);
            }
        }

        public virtual void VisitGetElementExpression(GetElementExpression node)
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
        public virtual void VisitSetElementExpression(SetElementExpression node)
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
        public virtual void VisitGetPropertyExpression(GetPropertyExpression node)
        {
            node.Object.Accept(this);
            node.Property.Accept(this);
        }

        public virtual void VisitSetPropertyExpression(SetPropertyExpression node)
        {
            node.Object.Accept(this);
            // Compile the value expression
            node.Value.Accept(this);
            node.Property.Accept(this);
        }


        public virtual void VisitMapExpression(MapExpression node)
        {
            foreach (var entry in node.ChildNodes)
            {
                if (entry is MapKeyValueExpression property)
                {
                    property.Value.Accept(this);
                }
            }
        }

        public virtual void VisitDeconstructionExpression(DeconstructionExpression node)
        {

        }
        public virtual void VisitBreakExpression(BreakStatement node)
        {

        }
        public virtual void VisitContinueExpression(ContinueStatement node)
        {

        }


        public virtual void VisitParameterDeclaration(ParameterDeclaration node)
        {
            node.Initializer?.Accept(this);
        }


    }
}
