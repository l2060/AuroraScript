using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;
using AuroraScript.Core;
using AuroraScript.Tokens;
using System.Collections.Generic;

namespace AuroraScript.Compiler.Emits
{
    /// <summary>
    /// 变量捕获器，用于分析函数中使用的变量，特别是闭包中捕获的外部变量
    /// </summary>
    public class VariableCatcher : IAstVisitor
    {
        // 存储所有引用的变量名
        public HashSet<String> Variables { get; } = new HashSet<String>();

        // 存储当前作用域中声明的变量名
        private HashSet<String> _declaredVariables = new HashSet<String>();

        // 存储捕获的变量（在外部作用域中定义但在当前函数中使用的变量）
        public HashSet<String> CapturedVariables { get; } = new HashSet<String>();

        // 存储嵌套函数声明，用于跟踪嵌套闭包
        private List<FunctionDeclaration> _nestedFunctions = new List<FunctionDeclaration>();

        /// <summary>
        /// 分析函数中的变量使用情况，识别捕获的变量
        /// </summary>
        /// <param name="functionNode">要分析的函数节点</param>
        /// <param name="parentScope">父作用域中的变量</param>
        /// <returns>捕获的变量列表</returns>
        public HashSet<String> AnalyzeFunction(FunctionDeclaration functionNode, CodeScope parentScope)
        {
            // 清除之前的分析结果
            Variables.Clear();
            _declaredVariables.Clear();
            CapturedVariables.Clear();
            _nestedFunctions.Clear();

            // 记录函数参数作为已声明的变量
            foreach (var param in functionNode.Parameters)
            {
                _declaredVariables.Add(param.Name.Value);
            }

            // 分析函数体
            functionNode.Body?.Accept(this);

            // 处理嵌套函数
            foreach (var nestedFunc in _nestedFunctions)
            {
                var nestedCatcher = new VariableCatcher();
                // 将当前函数的声明变量作为父作用域传递给嵌套函数
                var funcScope =  parentScope.Enter(null, DomainType.Function);
                nestedCatcher.AnalyzeFunction(nestedFunc, funcScope);
              
                // 将嵌套函数捕获的变量添加到当前函数的变量引用中
                foreach (var capturedVar in nestedCatcher.CapturedVariables)
                {
                    if (!_declaredVariables.Contains(capturedVar))
                    {
                        Variables.Add(capturedVar);
                        if (parentScope != null && parentScope.Resolve(capturedVar, out var _))
                        {
                            CapturedVariables.Add(capturedVar);
                        }
                    }
                }
            }

            // 确定捕获的变量：在函数中使用但不是在函数中声明的变量
            if (parentScope != null)
            {
                foreach (var variable in Variables)
                {
                    if (!_declaredVariables.Contains(variable) && parentScope.Resolve(variable,out var _))
                    {
                        CapturedVariables.Add(variable);
                    }
                }
            }

            return CapturedVariables;
        }

        public override void VisitVarDeclaration(VariableDeclaration node)
        {
            // 记录变量声明
            _declaredVariables.Add(node.Name.Value);

            // 处理初始化表达式
            base.VisitVarDeclaration(node);
        }

        public override void VisitParameterDeclaration(ParameterDeclaration node)
        {
            // 记录参数声明
            _declaredVariables.Add(node.Name.Value);

            // 处理默认值表达式
            base.VisitParameterDeclaration(node);
        }

        public override void VisitFunction(FunctionDeclaration node)
        {
            // 记录函数声明
            if (node.Name != null)
            {
                _declaredVariables.Add(node.Name.Value);
            }

            // 记录嵌套函数，稍后单独分析
            _nestedFunctions.Add(node);

            // 不立即分析函数体，避免作用域混淆
            // 不调用 base.VisitFunction(node)
        }

        public override void VisitLambdaExpression(LambdaExpression node)
        {
            // 记录嵌套函数，稍后单独分析
            _nestedFunctions.Add(node.Function);

            // 不立即分析函数体，避免作用域混淆
            // 不调用 base.VisitLambdaExpression(node)
        }

        public override void VisitName(NameExpression node)
        {
            // 记录变量引用
            string varName = node.Identifier.Value;

            // 忽略 this 和 global 关键字
            if (varName != "this" && varName != "global")
            {
                Variables.Add(varName);
            }
        }

        public override void VisitBlock(BlockStatement node)
        {
            // 创建块级作用域的变量跟踪
            HashSet<String> previousDeclaredVars = new HashSet<String>(_declaredVariables);

            // 处理块中的语句
            foreach (var statement in node.ChildNodes)
            {
                statement.Accept(this);
            }

            foreach (var item in node.Functions)
            {
                item.Accept(this);
            }
            

            // 不处理块中的函数声明，它们已经被添加到 _nestedFunctions

            // 恢复之前的作用域（块级变量离开作用域）
            if (node.IsNewScope)
            {
                _declaredVariables = previousDeclaredVars;
            }
        }

        public override void VisitForStatement(ForStatement node)
        {
            // 创建循环作用域的变量跟踪
            HashSet<String> previousDeclaredVars = new HashSet<String>(_declaredVariables);

            // 处理循环的各个部分
            base.VisitForStatement(node);

            // 恢复之前的作用域（循环变量离开作用域）
            _declaredVariables = previousDeclaredVars;
        }

        public override void VisitWhileStatement(WhileStatement node)
        {
            // 处理 while 循环
            base.VisitWhileStatement(node);
        }

        public override void VisitIfStatement(IfStatement node)
        {
            // 处理 if 语句
            base.VisitIfStatement(node);
        }

        public override void VisitCallExpression(FunctionCallExpression node)
        {
            // 处理函数调用
            base.VisitCallExpression(node);
        }

        public override void VisitAssignmentExpression(AssignmentExpression node)
        {
            // 处理赋值表达式
            // 先处理右侧，因为它可能引用变量
            node.Right.Accept(this);

            // 处理左侧，它可能是一个变量名
            if (node.Left is NameExpression nameExpr)
            {
                // 如果是赋值给变量，不将其视为变量引用
                // 但如果变量未声明，则可能是对外部变量的引用
                if (!_declaredVariables.Contains(nameExpr.Identifier.Value))
                {
                    Variables.Add(nameExpr.Identifier.Value);
                }
            }
            else
            {
                // 如果左侧不是简单的变量名，则处理它
                node.Left.Accept(this);
            }
        }
    }
}
