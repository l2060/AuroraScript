using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 成员函数表达式
    /// </summary>
    internal class GetPropertyExpression : OperatorExpression
    {
        internal GetPropertyExpression(Operator @operator) : base(@operator)
        {
        }

        public Expression Property
        {
            get
            {
                return this.childrens[1] as Expression;
            }
        }

        /// <summary>
        /// super object name
        /// </summary>
        public Expression Object
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }


        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptGetPropertyExpression(this);
        }

        public override string ToString()
        {
            return $"{Object}.{Property}";
        }
    }
}