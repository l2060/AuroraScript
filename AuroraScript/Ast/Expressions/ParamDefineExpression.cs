using AuroraScript.Stream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    internal class ParamDefineExpression : BinaryExpression
    {
        internal ParamDefineExpression(Operator @operator) : base(@operator)
        {
        }



        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            var isPriority = false;
            if (this.Parent is BinaryExpression parent)
            {
                isPriority = parent.Operator.Precedence > this.Operator.Precedence;
            }
            if (isPriority) writer.Write(Symbols.PT_LEFTPARENTHESIS.Name);
            this.Left.GenerateCode(writer);
            writer.Write($"{this.Operator.Symbol.Name} ");
            this.Right.GenerateCode(writer);
            if (isPriority) writer.Write(Symbols.PT_RIGHTPARENTHESIS.Name);
        }


    }
}
