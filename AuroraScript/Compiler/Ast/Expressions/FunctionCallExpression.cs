using AuroraScript.Compiler;
using System;
using System.Collections.Generic;


namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 函数调用
    /// </summary>
    internal class FunctionCallExpression : OperatorExpression
    {
        internal FunctionCallExpression(Operator @operator) : base(@operator)
        {
            this.Arguments = new List<Expression>();
        }

        public List<Expression> Arguments;


        public void AddArgument(Expression expression)
        {
            this.Arguments.Add(expression);
            expression.Parent = this;
        }



        public Expression Target
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }




        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptCallExpression(this);
        }


        public override string ToString()
        {
            return $"{Target}({String.Join(", ", Arguments)})";
        }

    }
}