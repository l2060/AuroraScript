using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// Postfix Expression
    /// i++
    /// i--
    /// </summary>
    public class PostfixExpression : OperatorExpression
    {
        internal PostfixExpression(Operator @operator) : base(@operator)
        {
        }

        public Exception Operand { get; set; }

        public Expression Left
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            this.Left.GenerateCode(writer);
            writer.Write(this.Operator.Symbol.Name);
        }
    }
}