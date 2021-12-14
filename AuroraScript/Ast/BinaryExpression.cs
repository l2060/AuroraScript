using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    /// <summary>
    /// 二元表达式
    /// </summary>
    internal class BinaryExpression:Expression
    {
        public Token Operator { get; set; }

        public AstNode Left { get; set; }

        public AstNode Right { get; set; }
    }
}
