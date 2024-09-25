using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 二元表达式
    /// </summary>
    public class BinaryExpression : OperatorExpression
    {
        internal BinaryExpression(Operator @operator) : base(@operator)
        {
        }

        //public Token Operator { get; set; }

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
            var isPriority = false;
            if (this.Parent is BinaryExpression parent)
            {
                isPriority = parent.Operator.Precedence > this.Operator.Precedence;
            }
            if (isPriority) writer.Write(Symbols.PT_LEFTPARENTHESIS.Name);
            this.Left.GenerateCode(writer);
            writer.Write($" {this.Operator.Symbol.Name} ");
            this.Right.GenerateCode(writer);
            if (isPriority) writer.Write(Symbols.PT_RIGHTPARENTHESIS.Name);
        }
    }
}