using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast
{
    /// <summary>
    /// function parameter declaration
    /// </summary>
    public class ParameterDeclaration : AstNode
    {
        internal ParameterDeclaration()
        {
        }

        /// <summary>
        /// parameter Modifier  ....
        /// </summary>
        public Token Modifier { get; set; }

        /// <summary>
        /// Parameter
        /// </summary>
        public Token Variable { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Expression DefaultValue { get; set; }

        /// <summary>
        /// 扩展运算符（Spread Operator）
        /// </summary>
        public Boolean IsSpreadOperator { get; set; } = false;


        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            if (IsSpreadOperator) writer.Write(Symbols.OP_SPREAD.Name);
            writer.Write(Variable.Value);
            if (DefaultValue != null)
            {
                writer.Write($" {Symbols.OP_ASSIGNMENT.Name} ");
                DefaultValue.GenerateCode(writer, depth);
            }
        }

        public override void Accept(IAstVisitor visitor)
        {
        }
    }
}