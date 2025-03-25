using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    public enum UnaryType
    {
        Prefix ,
        Post
    }



    /// <summary>
    /// Postfix Expression
    /// i++
    /// i--
    /// </summary>
    public class UnaryExpression : OperatorExpression
    {
        public UnaryType Type { get; private set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitUnaryExpression(this);
        }

        internal UnaryExpression(Operator @operator, UnaryType type) : base(@operator)
        {
            Type = type;
        }

        public Exception Operand { get; set; }

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
                return this.childrens[0] as Expression;
            }
        }

        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            if (Type == UnaryType.Post)
            {
                this.Left.GenerateCode(writer);
                writer.Write(this.Operator.Symbol.Name);
            }
            else
            {
                writer.Write(this.Operator.Symbol.Name);
                this.Right.GenerateCode(writer);
            }




        }
    }
}