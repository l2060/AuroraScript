
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

        public override String ToString()
        {
            return $"{Left} {this.Operator.Symbol.Name} {Right}";
        }


        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            this.Left.WriteCode(writer);
            writer.Write($" {this.Operator.Symbol.Name} ");
            this.Right.WriteCode(writer);

        }
    }
}
