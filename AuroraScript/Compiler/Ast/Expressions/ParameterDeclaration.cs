using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;
using System;


namespace AuroraScript.Ast
{
    /// <summary>
    /// function parameter declaration
    /// </summary>
    public class ParameterDeclaration : VariableDeclaration
    {
        internal ParameterDeclaration(Byte index, Token name, Expression defaultValue) : base(MemberAccess.Internal, false, name)
        {
            Name = name;
            //DefaultValue = defaultValue;
            Index = index;
            //Initializer = defaultValue;
            if (defaultValue != null)
            {
                this.AddNode(defaultValue);
            }

        }

        public Byte Index { get; set; }


        /// <summary>
        ///
        /// </summary>
        //public Expression DefaultValue { get; set; }

        /// <summary>
        /// 扩展运算符（Spread Operator）
        /// </summary>
        public Boolean IsSpreadOperator { get; set; } = false;

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitParameterDeclaration(this);
        }
    }
}