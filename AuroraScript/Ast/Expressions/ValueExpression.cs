
using AuroraScript.Tokens;

namespace AuroraScript.Ast.Expressions
{
    public class ValueExpression: Expression
    {

        internal ValueExpression(ValueToken value)
        {
            this.Value = value;
        }
        public ValueToken Value { get; set; }


        public override String ToString()
        {
            return $"{Value.ToValue()}";
        }

        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.Write(Value.ToValue());
        }

    }
}
