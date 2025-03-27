using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    public class NameExpression : Expression
    {
        /// <summary>
        /// member name
        /// </summary>
        public Token Identifier { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitName(this);
        }
        public override string ToString()
        {
            return this.Identifier.Value;
        }
    }
}