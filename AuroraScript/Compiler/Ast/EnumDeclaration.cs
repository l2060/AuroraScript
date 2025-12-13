using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using System;
using System.Collections.Generic;

namespace AuroraScript.Ast
{
    internal class EnumElement
    {
        public Token Name;
        public Int32 Value;
    }

    internal class EnumDeclaration : Statement
    {
        internal EnumDeclaration()
        {
            //this.Access = Symbols.KW_INTERNAL;
        }

        /// <summary>
        /// Function Access
        /// </summary>
        public Symbols Access { get; set; }

        public Token Identifier { get; set; }
        public List<EnumElement> Elements { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
        }
    }
}