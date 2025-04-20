using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;

namespace AuroraScript.Compiler.Ast.Statements
{

    public class ForInStatement : Statement
    {
        internal ForInStatement(VariableDeclaration initializer, InExpression iterator, Statement body)
        {
            Initializer = initializer;
            Iterator = iterator;
            Body = body;
        }

        /// <summary>
        /// for in initializer
        /// may be assignment
        /// may be variable declaration
        /// </summary>
        public VariableDeclaration Initializer { get; set; }

        public InExpression Iterator { get; set; }

        public Statement Body { get; set; }


        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitForInStatement(this);
        }
    }
}
