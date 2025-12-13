using AuroraScript.Compiler;
using System;


namespace AuroraScript.Ast.Expressions
{
    internal class NameExpression : Expression
    {
        /// <summary>
        /// member name
        /// </summary>
        public Token Identifier { get; set; }

        public Boolean IsRoot { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptName(this);
        }
        public override string ToString()
        {
            return this.Identifier.Value;
        }
    }
}