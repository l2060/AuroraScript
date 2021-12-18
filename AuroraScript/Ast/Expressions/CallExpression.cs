using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 函数调用
    /// </summary>
    internal class CallExpression: OperatorExpression
    {
        public CallExpression() : base(Operator.FunctionCall)
        {
        }

        public List<Expression> Arguments { get; set; }
        public Expression Base { get; set; }


    }
}
