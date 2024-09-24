

using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 成员函数表达式
    /// </summary>
    public class MemberAccessExpression : OperatorExpression
    {
        internal MemberAccessExpression(Operator @operator) : base(@operator)
        {

        }

        public Expression Property
        {
            get
            {
                return this.childrens[1] as Expression;
            }
        }


        /// <summary>
        /// super object name
        /// </summary>
        public Expression Object
        {
            get
            {
                return this.childrens[0] as Expression;
            }
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            IDisposable disposable = null;
            this.Object.GenerateCode(writer);
            if (Object is FunctionCallExpression fce)
            {
                writer.WriteLine();
                // 链式访问对齐
                disposable = writer.IncIndented();
            }
            writer.Write(Operator.MemberAccess.Symbol.Name);
            this.Property.GenerateCode(writer);
            if (disposable != null) disposable.Dispose();
        }
    }

}
