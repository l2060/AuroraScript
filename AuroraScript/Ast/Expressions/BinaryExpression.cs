using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 二元表达式
    /// </summary>
    internal class BinaryExpression:Expression
    {
        public Token Operator { get; set; }

        public Expression Left { get; set; }

        public Expression Right { get; set; }
    }
}
