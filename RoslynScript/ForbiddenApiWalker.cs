using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace RoslynScript
{


    public class ForbiddenSymbols
    {
        public String Method;
        public String Typed;
    }




    // 自定义的 Roslyn 语法树 Walker，用于检测不允许的 API 调用
    public class ForbiddenApiWalker : CSharpSyntaxWalker
    {

        public readonly List<String> ForbiddenApis = new List<String>();


        public bool HasForbiddenApis
        {
            get
            {
                return ForbiddenApis.Count > 0;
            }
        }

        private List<ForbiddenSymbols> forbiddenSymbols = new List<ForbiddenSymbols>()
        {
            new ForbiddenSymbols(){ Method = "Load", Typed = "Assembly"},
            new ForbiddenSymbols(){ Method = "LoadFrom", Typed = "Assembly"},
            new ForbiddenSymbols(){ Method = "Start", Typed = "Process"},
        };



        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var expression = node.Expression as MemberAccessExpressionSyntax;
            if (expression != null)
            {
                var methodName = expression.Name.Identifier.Text;
                var typeName = expression.Expression.ToString();
                var symbols = forbiddenSymbols.Find(e => e.Method == methodName && e.Typed == typeName);
                if (symbols != null)
                {
                    this.AddReport(symbols, node);
                }
            }
            base.VisitInvocationExpression(node);
        }

        private void AddReport(ForbiddenSymbols symbols, InvocationExpressionSyntax node)
        {
            // 获取位置并打印行列信息
            var location = node.GetLocation();
            var lineSpan = location.GetLineSpan();
            // 输出错误信息，包含行列
            ForbiddenApis.Add($"({lineSpan.StartLinePosition.Line + 1},{lineSpan.StartLinePosition.Character + 1}) Error: Forbidden API '{symbols.Typed}.{symbols.Method}' detected in script.");
        }

    }
}
