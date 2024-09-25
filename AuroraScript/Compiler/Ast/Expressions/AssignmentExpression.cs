using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 赋值
    /// </summary>
    public class AssignmentExpression : OperatorExpression
    {
        internal AssignmentExpression(Operator @operator) : base(@operator)
        {
        }

        public Expression Left
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }

        public Expression Right
        {
            get
            {
                return this.childrens[1] as Expression;
            }
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            this.Left.GenerateCode(writer);
            writer.Write($" {this.Operator.Symbol.Name} ");
            this.Right.GenerateCode(writer);
        }
    }
}