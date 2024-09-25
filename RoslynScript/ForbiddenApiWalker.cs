using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace RoslynScript
{
    // 自定义的 Roslyn 语法树 Walker，用于检测不允许的 API 调用
    public class ForbiddenApiWalker : CSharpSyntaxWalker
    {
        public bool HasForbiddenApis { get; private set; } = false;

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var expression = node.Expression as MemberAccessExpressionSyntax;
            if (expression != null)
            {
                var methodName = expression.Name.Identifier.Text;
                var typeName = expression.Expression.ToString();

                // 检测是否调用了 Assembly.LoadFrom
                if ((methodName == "Load" || methodName == "LoadFrom") && typeName == "Assembly")
                {
                    HasForbiddenApis = true;
                }

                if ((methodName == "Start") && typeName == "Process")
                {
                    HasForbiddenApis = true;
                }


            }

            base.VisitInvocationExpression(node);
        }
    }
}
