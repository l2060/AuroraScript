using AuroraScript.Ast.Expressions;
using AuroraScript.Exceptions;
using System.Linq.Expressions;

namespace AuroraScript.Ast.Statements
{
    /// <summary>
    /// Coroutine Statement
    /// </summary>
    public class CoroutineExpression : OperatorExpression
    {
        internal CoroutineExpression(Operator @operator) : base(@operator)
        {

        }

        public Exception Operand { get; set; }

        public FunctionCallExpression FunctionCall
        {
            get
            {
                if(this.childrens[0] is FunctionCallExpression exp)
                {
                    return exp;
                }
                throw new Exception("Coroutine operator arguments must be method calls");
                //return this.childrens[0] as FunctionCallExpression;
            }
        }

        public override String ToString()
        {
            return $"{Operator.Coroutine.Symbol.Name} {this.FunctionCall}";
        }

        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.Write($"{Operator.Coroutine.Symbol.Name} ");
            this.FunctionCall.WriteCode(writer);

        }





        public override void AddNode(AstNode node)
        {
            if (this.childrens.Count > 0) throw new Exception("The coroutine operator supports only one argument");
            //if(node is FunctionCallExpression)
            //{
                base.AddNode(node);
            //    return;
            //}
            //throw new Exception("Coroutine operator arguments must be method calls");

        }

    }
}
