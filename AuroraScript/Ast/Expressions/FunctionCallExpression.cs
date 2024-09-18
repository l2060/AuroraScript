

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 函数调用
    /// </summary>
    public class FunctionCallExpression : OperatorExpression
    {
        internal FunctionCallExpression(Operator @operator) : base(@operator)
        {
            this.Arguments = new List<Expression>();
        }

        public List<Expression> Arguments { get; set; }
        public Expression Target
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }


        public override String ToString()
        {
            return $"{Target}{Operator.FunctionCall.Symbol.Name}{String.Join(',', Arguments.Select(e => e.ToString()))}{Operator.FunctionCall.SecondarySymbols.Name}";
        }


        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            Target.WriteCode(writer);
            writer.Write(Operator.FunctionCall.Symbol.Name);
            this.writeParameters(writer, Arguments, ", ");
            writer.Write(Operator.FunctionCall.SecondarySymbols.Name);
        }

    }
}
