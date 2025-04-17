using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using AuroraScript.Compiler.Ast;
using System;


namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// variable declaration
    /// </summary>
    public class VariableDeclaration : Statement, INamedStatement
    {
        internal VariableDeclaration(MemberAccess access, Boolean isConst, Token nameToken)
        {
            Access = access;
            IsConst = isConst;
            Name = nameToken;
        }
        /// <summary>
        /// parameter Modifier  ....
        /// </summary>
        public Token Modifier { get; set; }

        /// <summary>
        /// variable names
        /// </summary>
        public Token Name { get; set; }

        /// <summary>
        /// var initialize statement
        /// </summary>
        public Expression Initializer => ChildNodes.Count > 0 ? ChildNodes[0] as Expression : null;

        /// <summary>
        /// Function Access
        /// </summary>
        public MemberAccess Access { get; set; }

        /// <summary>
        /// this variable use const declare
        /// </summary>
        public bool IsConst { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitVarDeclaration(this);
        }

        public override string ToString()
        {
            var export = "";
            if (Access == MemberAccess.Export) export = "export ";
            return $"{export}var {Name.Value} = {Initializer}";
        }


    }
}