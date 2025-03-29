using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;


namespace AuroraScript.Ast
{
    /// <summary>
    /// function parameter declaration
    /// </summary>
    public class ParameterDeclaration : Statement
    {
        internal ParameterDeclaration(int index, Token name, Expression defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
            Index = index;
        }

        public int Index { get; set; }

        /// <summary>
        /// Parameter
        /// </summary>
        public Token Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Expression DefaultValue { get; set; }

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