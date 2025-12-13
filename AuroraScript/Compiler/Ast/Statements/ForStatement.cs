using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler;


namespace AuroraScript.Ast.Statements
{
    internal class ForStatement : Statement
    {
        internal ForStatement(Expression condition, AstNode initializer, Expression incrementor, Statement body)
        {
            Condition = condition;
            Initializer = initializer;
            Incrementor = incrementor;
            Body = body;
        }

        public Expression Condition { get; set; }

        public Statement Body { get; set; }

        /// <summary>
        /// for initializer
        /// may be assignment
        /// may be variable declaration
        /// </summary>
        public AstNode Initializer { get; set; }

        /// <summary>
        /// for incrementor
        /// contains multiple sentences
        /// </summary>
        public Expression Incrementor { get; set; }


        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptForStatement(this);
        }
    }
}