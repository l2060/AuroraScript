using AuroraScript.Compiler;
using System;


namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// PrefixUnary Expression
    /// ++i
    /// --i
    /// </summary>
    internal abstract class PrefixUnaryExpression : OperatorExpression
    {
        internal PrefixUnaryExpression(Operator @operator) : base(@operator)
        {
        }

        public Exception Operand { get; set; }

        public Expression Right
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }

    }
}