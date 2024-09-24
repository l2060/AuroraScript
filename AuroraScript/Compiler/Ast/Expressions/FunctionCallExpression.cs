

using AuroraScript.Compiler;
using AuroraScript.Stream;

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


        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            Target.GenerateCode(writer);
            writer.Write(Operator.FunctionCall.Symbol.Name);
            this.writeParameters(writer, Arguments, Symbols.PT_COMMA.Name + " ");
            writer.Write(Operator.FunctionCall.SecondarySymbols.Name);
        }

    }
}
